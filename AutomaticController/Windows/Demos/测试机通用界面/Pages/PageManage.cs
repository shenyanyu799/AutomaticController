using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    public class PageManage
    {
        /// <summary>
        /// 口令有效时间
        /// </summary>
        public double UnockTime = 300;
        /// <summary>
        /// 至少成功切换过一次页面()
        /// </summary>
        public bool IsNext { get; private set; }
        Dictionary<string, Page> Pages = new Dictionary<string, Page>();
        Frame theFrame;
        string pageName;
        //上次输入密码的时间
        DateTime lastkeyTime;
        public double RemainingLockTime => UnockTime - (DateTime.Now - lastkeyTime).TotalSeconds;
        public PageManage(Frame frame)
        {
            theFrame = frame;
        }
        public void Add(string name, Page page)
        {
            Pages.Add(name, page);
        }
        public void SetKeyPage(KeyPage Page)
        {
            this.keyPage = Page;
        }
        KeyPage keyPage;
        public void Next(string name)
        {
            Page page = null;
            if (Pages.TryGetValue(name, out page))
            {
                if (theFrame.Content != page)
                {
                    pageName = name;
                    theFrame.Navigate(page);
                    IsNext = true;
                }

            }
        }
        public bool IsPage(string name) => name == pageName;
        public bool IsUnlock(string key) => (key == null || key.Length == 0 || (DateTime.Now - lastkeyTime).TotalSeconds <= UnockTime);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">需要验证的口令</param>
        /// <param name="name"></param>
        public void NextKey(string key, string name)
        {
            Page page = null;
            if (Pages.TryGetValue(name, out page))
            {
                IsNext = true;
                pageName = name;
                if (theFrame.Content != page && keyPage != null)
                {
                    if (IsUnlock(key))
                    {
                        theFrame.Navigate(page);
                        return;
                    }
                    keyPage.Verify = k =>
                    {
                        if (k == key)
                        {
                            theFrame.Navigate(page);
                            lastkeyTime = DateTime.Now;
                        }
                    };

                    keyPage.Cancel = () =>
                    {
                        theFrame.GoBack();
                    };
                    theFrame.Navigate(keyPage);

                }

            }

        }
    }
    public class KeyPage : Page
    {
        /// <summary>
        /// 确认
        /// </summary>
        public Action<string> Verify { get; set; }
        /// <summary>
        /// 取消
        /// </summary>
        public Action Cancel { get; set; }
    }
}
