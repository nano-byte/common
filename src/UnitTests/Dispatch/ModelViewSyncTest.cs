// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NanoByte.Common.Collections;
using Xunit;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Contains test methods for <see cref="ModelViewSync{TModel,TView}"/>.
    /// </summary>
    public class ModelViewSyncTest
    {
        #region Mock classes
        private abstract class ModelBase : IChangeNotify<ModelBase>
        {
            private string? _id;

            public string? ID
            {
                get => _id;
                set
                {
                    _id = value;
                    Changed?.Invoke(this);
                }
            }

            public void Rebuild() => ChangedRebuild?.Invoke(this);

            public event Action<ModelBase>? Changed;
            public event Action<ModelBase>? ChangedRebuild;
        }

        private class SpecificModel : ModelBase
        {}

        private abstract class ViewBase : IDisposable
        {
            public string? ID { get; set; }

            public bool Disposed { get; private set; }

            public void Dispose() => Disposed = true;
        }

        private class SpecificView : ViewBase
        {}
        #endregion

        [Fact]
        public void Initialize()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            view[0].Should().BeOfType<SpecificView>();
            view[0].ID.Should().Be("abc");
        }

        [Fact]
        public void Lookup()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            sync.Lookup(view[0]).Should().BeSameAs(model[0]);
            sync.Invoking(x => x.Lookup(new SpecificView()))
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void Representations()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            sync.Representations.First().Should().BeSameAs(view[0]);
        }

        [Fact]
        public void MonitoringAdd()
        {
            var model = new MonitoredCollection<ModelBase>();
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            model.Add(new SpecificModel {ID = "abc"});

            view[0].Should().BeOfType<SpecificView>();
            view[0].ID.Should().Be("abc");
        }

        [Fact]
        public void MonitoringRemove()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            var originalRepresentation = view[0];
            model.RemoveAt(0);

            view.Should().BeEmpty();
            originalRepresentation.Disposed.Should().BeTrue();
        }

        [Fact]
        public void MonitoringChanged()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            model[0].ID = "xyz";

            view[0].ID.Should().Be("xyz");
        }

        [Fact]
        public void MonitoringChangedRebuild()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using var sync = new ModelViewSync<ModelBase, ViewBase>(model, view);
            sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
            sync.Initialize();

            var originalRepresentation = view[0];
            model[0].Rebuild();
            view[0].Should().NotBeSameAs(originalRepresentation);
        }

        [Fact]
        public void Dispose()
        {
            var model = new MonitoredCollection<ModelBase> {new SpecificModel {ID = "abc"}};
            var view = new List<ViewBase>();
            using (var sync = new ModelViewSync<ModelBase, ViewBase>(model, view))
            {
                sync.Register((SpecificModel _) => new SpecificView(), (element, representation) => representation.ID = element.ID);
                sync.Initialize();
            }
            view.Should().BeEmpty();
        }
    }
}
