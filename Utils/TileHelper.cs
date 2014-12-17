using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NotificationsExtensions.TileContent;
using Windows.UI.Notifications;
using NotificationsExtensions.BadgeContent;

namespace Weather.Utils
{
    /// <summary>
    /// 磁贴类
    /// </summary>
    public class TileHelper
    {






        //public static CreateSecondaryTile(string tileId)
        //{

        //}






        /// <summary>
        /// 更新默认磁贴内容(不通用)
        /// </summary>
        /// <param name="tileModel"></param>
        public static void UpdateTileNotifications(string ImageSrc, string TextHeading, string TextBody1, string TextBody2, string TextBody3)
        {

            ITileWide310x150ImageAndText01 tileWide310x150ImageAndText01 = TileContentFactory.CreateTileWide310x150ImageAndText01();

            ITileSquare150x150PeekImageAndText01 tileSquare150x150PeekImageAndText01 = TileContentFactory.CreateTileSquare150x150PeekImageAndText01();
            //磁铁内容赋值
            tileSquare150x150PeekImageAndText01.Image.Src = ImageSrc;
            tileSquare150x150PeekImageAndText01.TextHeading.Text = TextHeading;
            tileSquare150x150PeekImageAndText01.TextBody1.Text = TextBody1;
            tileSquare150x150PeekImageAndText01.TextBody2.Text = TextBody2;
            tileSquare150x150PeekImageAndText01.TextBody3.Text = TextBody3;
            tileWide310x150ImageAndText01.Square150x150Content = tileSquare150x150PeekImageAndText01;
            tileWide310x150ImageAndText01.Image.Src = "ms-appx:///Assets/WideLogo.scale-100.png";
            tileWide310x150ImageAndText01.TextCaptionWrap.Text = TextHeading + TextBody1 + TextBody2 + TextBody3;
            //更新至磁贴
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileWide310x150ImageAndText01.CreateNotification());
        }





        /// <summary>
        /// 通过Xml磁贴模版文件更新默认磁贴内容
        /// </summary>
        /// <param name="tileXml"></param>
        public static void UpdateTileNotificationsByXml(string tileXmlString)
        {

            /*
            string tileXmlString = "<tile>"
                + "<visual version='3'>"
                + "<binding template='TileSquare71x71Image'>"
                + "<image id='1' src='ms-appx:///images/graySquare150x150.png' alt='Gray image'/>"
                + "</binding>"
                + "<binding template='TileSquare150x150Image' fallback='TileSquareImage'>"
                + "<image id='1' src='ms-appx:///images/graySquare150x150.png' alt='Gray image'/>"
                + "</binding>"
                + "<binding template='TileWide310x150ImageAndText01' fallback='TileWideImageAndText01'>"
                + "<image id='1' src='ms-appx:///images/redWide310x150.png' alt='Red image'/>"
                + "<text id='1'>This tile notification uses ms-appx images</text>"
                + "</binding>"
                + "<binding template='TileSquare310x310Image'>"
                + "<image id='1' src='ms-appx:///images/purpleSquare310x310.png' alt='Purple image'/>"
                + "</binding>"
                + "</visual>"
                + "</tile>";
             * */

            // Create a DOM.
            Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();

            // Load the xml string into the DOM.
            tileDOM.LoadXml(tileXmlString);

            // Create a tile notification.
            TileNotification tile = new TileNotification(tileDOM);

            // Send the notification to the application’s tile.
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
        }

        /// <summary>
        /// 更新磁贴数字
        /// </summary>
        /// <param name="number"></param>
        public static void UpdateBadgeWithNumber(int number)
        {
            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent((uint)number);

            // Send the notification to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());
        }

        /// <summary>
        /// 通过Xml磁贴模版文件更新磁贴数字
        /// </summary>
        /// <param name="number"></param>
        public static void UpdateBadgeWithNumberWithByXml(int number)
        {
            // Create a string with the badge template xml.
            string badgeXmlString = "<badge value='" + number + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            // Create a DOM.
            badgeDOM.LoadXml(badgeXmlString);

            // Load the xml string into the DOM, catching any invalid xml characters.
            BadgeNotification badge = new BadgeNotification(badgeDOM);

            // Create a badge notification and send it to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
        }

        /// <summary>
        /// 更新磁贴图标
        /// </summary>
        /// <param name="number"></param>
        public static void UpdateBadgeWithGlyph(int index)
        {
            BadgeGlyphNotificationContent badgeContent = new BadgeGlyphNotificationContent((GlyphValue)index);

            // Send the notification to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());
        }

        /// <summary>
        /// 通过Xml磁贴模版文件更新磁贴图标
        /// </summary>
        /// <param name="TileGlyph"></param>
        public static void UpdateBadgeWithGlyphByXml(string TileGlyph)
        {


            /*
                            public class TileGlyph
                {
                    public string Name { get; private set; }
                    public bool IsAvailable { get; private set; }
                    public TileGlyph(string name, bool isAvailable)
                    {
                        this.Name = name;
                        this.IsAvailable = isAvailable;
                    }
                    public override string ToString()
                    {
                        return Name;
                    }
                }

                public class TileGlyphCollection : ObservableCollection<TileGlyph>
                {
        
                    public TileGlyphCollection()
                    {
                        // Some glyphs are only available on Windows
            #if WINDOWS_PHONE_APP
                        const bool windows = false;
                        const bool phone = true;
            #else
                        const bool windows = true;
                        const bool phone = false;
            #endif

                        Add(new TileGlyph("none", windows | phone));
                        Add(new TileGlyph("activity", windows));
                        Add(new TileGlyph("alert", windows | phone));
                        Add(new TileGlyph("available", windows));
                        Add(new TileGlyph("away", windows));
                        Add(new TileGlyph("busy", windows));
                        Add(new TileGlyph("newMessage", windows));
                        Add(new TileGlyph("paused", windows));
                        Add(new TileGlyph("playing", windows));
                        Add(new TileGlyph("unavailable", windows));
                        Add(new TileGlyph("error", windows));
                        Add(new TileGlyph("attention", windows | phone));
                        Add(new TileGlyph("alarm", windows));
                    }
                }
                     * */

            // Create a string with the badge template xml.
            string badgeXmlString = "<badge value='" + TileGlyph + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();

            // Create a DOM.
            badgeDOM.LoadXml(badgeXmlString);

            // Load the xml string into the DOM, catching any invalid xml characters.
            BadgeNotification badge = new BadgeNotification(badgeDOM);

            // Create a badge notification and send it to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);


        }

        /// <summary>
        /// 启用磁贴队列，最多循环5个
        /// </summary>
        public static void EnableNotificationQueue()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }

        /// <summary>
        /// 禁用磁贴队列
        /// </summary>
        public static void DisableNotificationQueue()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(false);
        }

        /// <summary>
        /// 清除默认磁贴内容
        /// </summary>
        public static void CleanTileNotifications()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }


    }
}
