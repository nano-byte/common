// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls
{
    // ReSharper disable CommentTypo
    /// <summary>
    /// Mask indicating which fields in <see cref="TouchEventArgs"/> are valid.
    /// </summary>
    /// <seealso cref="TouchEventArgs.Mask"/>
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "The keyword mask implies flag usage without the need for a plural form")]
    [Flags]
    public enum TouchEventMask
    {
        /// <summary>TOUCHINPUTMASKF_TIMEFROMSYSTEM</summary>
        Time = 0x0001,

        /// <summary>TOUCHINPUTMASKF_EXTRAINFO</summary>
        ExtraInfo = 0x0002,

        /// <summary>TOUCHINPUTMASKF_CONTACTAREA</summary>
        ContactArea = 0x0004
    }

    /// <summary>
    /// Event information about a touch event.
    /// </summary>
    public class TouchEventArgs : EventArgs
    {
        /// <summary>
        /// Touch X client coordinate in pixels.
        /// </summary>
        public int LocationX { get; set; }

        /// <summary>
        /// Touch Y client coordinate in pixels.
        /// </summary>
        public int LocationY { get; set; }

        /// <summary>
        /// X size of the contact area in pixels.
        /// </summary>
        public int ContactX { get; set; }

        /// <summary>
        /// X size of the contact area in pixels.
        /// </summary>
        public int ContactY { get; set; }

        /// <summary>
        /// Contact ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mask indicating which fields in the structure are valid.
        /// </summary>
        public TouchEventMask Mask { get; set; }

        /// <summary>
        /// Touch event time.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Indicates that this structure corresponds to a primary contact point.
        /// </summary>
        public bool Primary;

        /// <summary>
        /// The touch event came from the user's palm.
        /// </summary>
        public bool Palm;

        /// <summary>
        /// The user is hovering above the touch screen.
        /// </summary>
        public bool InRange;

        /// <summary>
        /// This input was not coalesced.
        /// </summary>
        public bool NoCoalesce;
    }
}
