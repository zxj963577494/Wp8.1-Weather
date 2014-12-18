using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Weather.Utils
{
    /// <summary>
    /// 辅助磁贴
    /// </summary>
    public static class SecondaryTileHelper
    {
        private static Uri square150x150Logo = new Uri("ms-appx:///Assets/Logo.scale-100.png");
        private static Uri wide310x150Logo = new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
        private static Uri square310x310Logo = new Uri("ms-appx:///Assets/square310x310Tile-sdk.png");
        private static Uri square30x30Logo = new Uri("ms-appx:///Assets/square30x30Tile-sdk.png");

        /// <summary>
        /// 创建辅助磁贴
        /// </summary>
        /// <param name="tileId">磁贴的唯一 ID</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="tileActivationArguments">激活辅助磁贴时传递到父应用程序的参数字符串</param>
        public async static void CreateSecondaryTileAsync(string tileId, string displayName, string tileActivationArguments)
        {
            SecondaryTile secondaryTile = new SecondaryTile(tileId,
                                                displayName,
                                                tileActivationArguments,
                                                square150x150Logo,
                                                TileSize.Square150x150);
            secondaryTile.VisualElements.Square30x30Logo = square30x30Logo;
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await secondaryTile.RequestCreateAsync();
        }

        /// <summary>
        /// 移除辅助磁贴
        /// </summary>
        /// <param name="tileId">磁贴的唯一 ID</param>
        public async static void DeleteSecondaryTileAsync(string tileId)
        {
            if (Windows.UI.StartScreen.SecondaryTile.Exists(tileId))
            {
                // First prepare the tile to be unpinned
                SecondaryTile secondaryTile = new SecondaryTile(tileId);
                // Now make the delete request.
                await secondaryTile.RequestDeleteAsync();
            }
        }

        /// <summary>
        /// 检查是否存在磁贴
        /// </summary>
        /// <param name="tileId"></param>
        public static bool IsExists(string tileId)
        {
            bool isSuccess = Windows.UI.StartScreen.SecondaryTile.Exists(tileId);
            return isSuccess;
        }

        /// <summary>
        /// 获取所有辅助磁贴
        /// </summary>
        /// <returns>磁贴唯一ID集合</returns>
        public async static Task<List<string>> GetAllSecondaryTileAsync()
        {
            List<string> tileIdListString = new List<string>();
            IReadOnlyList<SecondaryTile> tilelist = await Windows.UI.StartScreen.SecondaryTile.FindAllAsync();
            if (tilelist.Count > 0)
            {
                foreach (var tile in tilelist)
                {
                    tileIdListString.Add(tile.TileId);
                }
            }
            return tileIdListString;
        }

        /// <summary>
        /// 更新默认磁贴内容(不通用)
        /// </summary>
        /// <param name="tileModel"></param>
        public static void UpdateSecondaryTileNotifications(string tileId, string ImageSrc, string TextHeading, string TextBody1, string TextBody2, string TextBody3)
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
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId).Update(tileWide310x150ImageAndText01.CreateNotification());
        }


        /// <summary>
        /// 通过Xml磁贴模版文件更新默认磁贴内容
        /// </summary>
        /// <param name="tileXml"></param>
        public static void UpdateSecondaryTileNotificationsByXml(string tileId, string tileXmlString)
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
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId).Update(tile);
        }

        /// <summary>
        /// 更新磁贴数字
        /// </summary>
        /// <param name="number"></param>
        public static void UpdateSecondaryBadgeWithNumber(string tileId, int number)
        {
            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent((uint)number);

            // Send the notification to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tileId).Update(badgeContent.CreateNotification());
        }

        /// <summary>
        /// 通过Xml磁贴模版文件更新磁贴数字
        /// </summary>
        /// <param name="number"></param>
        public static void UpdateSecondaryBadgeWithNumberWithByXml(string tileId, int number)
        {
            // Create a string with the badge template xml.
            string badgeXmlString = "<badge value='" + number + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            // Create a DOM.
            badgeDOM.LoadXml(badgeXmlString);

            // Load the xml string into the DOM, catching any invalid xml characters.
            BadgeNotification badge = new BadgeNotification(badgeDOM);

            // Create a badge notification and send it to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tileId).Update(badge);
        }

        /// <summary>
        /// 更新磁贴图标
        /// </summary>
        /// <param name="number"></param>
        public static void UpdateSecondaryBadgeWithGlyph(string tileId, int index)
        {
            BadgeGlyphNotificationContent badgeContent = new BadgeGlyphNotificationContent((GlyphValue)index);

            // Send the notification to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tileId).Update(badgeContent.CreateNotification());
        }

        /// <summary>
        /// 通过Xml磁贴模版文件更新磁贴图标
        /// </summary>
        /// <param name="TileGlyph"></param>
        public static void UpdateBadgeWithGlyphByXml(string tileId, string TileGlyph)
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
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tileId).Update(badge);


        }
    }
}
