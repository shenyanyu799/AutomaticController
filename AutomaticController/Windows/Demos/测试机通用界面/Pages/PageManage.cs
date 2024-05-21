﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    public class PageManage
    {
        /// <summary>
        /// 至少成功切换过一次页面()
        /// </summary>
        public bool IsNext{  get; private set; }
        Dictionary<string, Page> Pages = new Dictionary<string, Page>();
        Frame theFrame;
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
                    theFrame.Navigate(page);
                    IsNext = true;
                }

            }
        }

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
                if (theFrame.Content != page && keyPage != null)
                {
                    if(key == null ||  key.Length == 0)
                    {
                        theFrame.Navigate(page);
                        return;
                    }
                    keyPage.Verify = k =>
                    {
                        if (k == key)
                        {
                            theFrame.Navigate(page);
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
    public class KeyPage:Page
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
