// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Keeps two collections of different types in sync based on mapping rules.
    /// </summary>
    /// <typeparam name="TModel">The common base type of elements in the model.</typeparam>
    /// <typeparam name="TView">The common base type of representations in the view.</typeparam>
    /// <remarks>
    /// Useful for maintaining View representations for a set of mutable Model elements in a Model-View-Controller/Presenter design.
    /// Generated View representations will automatically be disposed on removal, if they implement <see cref="IDisposable"/>.
    /// </remarks>
    public sealed class ModelViewSync<TModel, TView> : IDisposable
        where TModel : class, IChangeNotify<TModel>
        where TView : class
    {
        private readonly MonitoredCollection<TModel> _model;
        private readonly ICollection<TView> _view;

        /// <summary>
        /// Creates a new Model-View synchronizer.
        /// </summary>
        /// <param name="model">The Model that can change on its own accord.</param>
        /// <param name="view">The View that is to be automatically updated to reflect changes in the Model.</param>
        public ModelViewSync(MonitoredCollection<TModel> model, ICollection<TView> view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        private bool _initialized;

        /// <summary>
        /// Must be called once, after all relevant mapping rules have been registered.
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;

            foreach (var element in _model)
                OnAdded(element);

            _model.Added += OnAdded;
            _model.Removed += OnRemoved;

            _initialized = true;
        }

        public void Dispose()
        {
            _initialized = false;

            _model.Added -= OnAdded;
            _model.Removed -= OnRemoved;

            foreach (var element in _model)
                OnRemoved(element);
        }

        private readonly MultiDictionary<TModel, TView> _modelToView = new MultiDictionary<TModel, TView>();

        private readonly Dictionary<TView, TModel> _viewToModel = new Dictionary<TView, TModel>();

        /// <summary>
        /// All View representations created by the synchronizer.
        /// </summary>
        public IEnumerable<TView> Representations => _modelToView.Values;

        private void OnAdded(TModel element)
        {
            element.Changed += OnChanged;
            element.ChangedRebuild += OnRemoved;
            element.ChangedRebuild += OnAdded;

            foreach (var representation in _createDispatcher.Dispatch(element))
            {
                _view.Add(representation);
                _modelToView.Add(element, representation);
                _viewToModel.Add(representation, element);
            }
            _updateDispatcher.Dispatch(element);
        }

        private void OnRemoved(TModel element)
        {
            element.ChangedRebuild -= OnAdded;
            element.ChangedRebuild -= OnRemoved;
            element.Changed -= OnChanged;

            foreach (var representation in _modelToView[element])
            {
                _view.Remove(representation);
                _viewToModel.Remove(representation);

                var disposable = representation as IDisposable;
                disposable?.Dispose();
            }
            _modelToView.Remove(element);
        }

        private void OnChanged(TModel element) => _updateDispatcher.Dispatch(element);

        /// <summary>
        /// Looks up the Model element a View representation was created for.
        /// </summary>
        /// <exception cref="KeyNotFoundException">There is no match.</exception>
        public TModel Lookup(TView representation) => _viewToModel[representation];

        private readonly AggregateDispatcher<TModel, TView> _createDispatcher = new AggregateDispatcher<TModel, TView>();

        private readonly AggregateDispatcher<TModel> _updateDispatcher = new AggregateDispatcher<TModel>();

        /// <summary>
        /// Registers a mapping rule for a specific type of Model element.
        /// </summary>
        /// <param name="create">Callback that creates a set of 0..n View representations for a given Model element.</param>
        /// <param name="update">Callback that updates a specific View representation based on the state of a given Model element; can be <c>null</c>.</param>
        public void RegisterMultiple<TSpecificModel, TSpecificView>(Func<TSpecificModel, IEnumerable<TSpecificView>> create, Action<TSpecificModel, TSpecificView>? update = null)
            where TSpecificModel : class, TModel
            where TSpecificView : class, TView
        {
            if (create == null) throw new ArgumentNullException(nameof(create));

#if NET20 || NET35
            _createDispatcher.Add<TSpecificModel>(element => create(element).OfType<TView>());
#else
            _createDispatcher.Add(create);
#endif
            if (update != null)
            {
                _updateDispatcher.Add((TSpecificModel element) =>
                {
                    foreach (var representation in _modelToView[element].OfType<TSpecificView>())
                        update(element, representation);
                });
            }
        }

        /// <summary>
        /// Registers a mapping rule for a specific type of Model element.
        /// </summary>
        /// <param name="create">Callback that creates a View representation for a given Model element.</param>
        /// <param name="update">Callback that updates a View representation based on the state of a given Model element; can be <c>null</c>.</param>
        public void Register<TSpecificModel, TSpecificView>(Func<TSpecificModel, TSpecificView> create, Action<TSpecificModel, TSpecificView>? update = null)
            where TSpecificModel : class, TModel
            where TSpecificView : class, TView
        {
            if (create == null) throw new ArgumentNullException(nameof(create));

            RegisterMultiple(element => new[] {create(element)}, update);
        }
    }
}
