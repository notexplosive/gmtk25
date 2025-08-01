using System;
using System.Collections.Generic;
using System.Linq;
using OutLoop.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class AppController : MonoBehaviour
    {
        [SerializeField]
        private AppButtonRow? _appButtonRow;

        [SerializeField]
        private List<AppPage> _pages = new();

        [SerializeField]
        private LoopDataRelay? _relay;
        
        private AppPage? _currentPage;

        private void Awake()
        {
            if (_appButtonRow == null)
            {
                Debug.Log("Missing button row");
                return;
            }

            if (_pages.Count < 4)
            {
                Debug.Log($"Not enough pages, only found {_pages.Count}");
                return;
            }

            _currentPage = _pages.First();
            
            foreach (var page in _pages)
            {
                if (page != _currentPage)
                {
                    page.gameObject.SetActive(false);
                }
            }

            if (_appButtonRow.HomeButton != null)
            {
                _appButtonRow.HomeButton.Clicked += CreatePageToEvent(_pages[0]);
            }

            if (_appButtonRow.SearchButton != null)
            {
                _appButtonRow.SearchButton.Clicked += CreatePageToEvent(_pages[1]);
            }

            if (_appButtonRow.MessagesButton != null)
            {
                _appButtonRow.MessagesButton.Clicked += CreatePageToEvent(_pages[2]);
            }

            if (_appButtonRow.FavoritesButton != null)
            {
                _appButtonRow.FavoritesButton.Clicked += CreatePageToEvent(_pages[3]);
            }
        }

        private Action CreatePageToEvent(AppPage page)
        {
            return () => { PageTo(page); };
        }

        private void PageTo(AppPage page)
        {
            if (_currentPage == page)
            {
                return;
            }

            if (_currentPage == null)
            {
                _currentPage = page;
                page.FlyIn(1);
                return;
            }

            var targetPageIndex = _pages.IndexOf(page);
            var currentPageIndex = _pages.IndexOf(_currentPage);
            var direction = Math.Sign(targetPageIndex - currentPageIndex);
            
            _currentPage.FlyOut(direction);
            page.FlyIn(direction);
            _currentPage = page;

            if (_relay != null)
            {
                _relay.State().CurrentPage = page.PageType;
            }
        }
    }
}