using System;
using System.Collections.Generic;

namespace ccc.Services
{
    public class NavigationService
    {
        public event EventHandler<string>? PageChanged;

        private string _currentPage = "playlists";
        private readonly Stack<string> _history = new Stack<string>();

        public string CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    PageChanged?.Invoke(this, value);
                }
            }
        }

        public void NavigateTo(string pageKey)
        {
            if (_currentPage == pageKey) return;

            // Push current to history before moving
            _history.Push(_currentPage);
            CurrentPage = pageKey;
        }

        public void GoBack()
        {
            if (_history.Count > 0)
            {
                string previousPage = _history.Pop();
                // We set internal field directly or use a variant of Navigate that doesn't push?
                // Actually, logic says: restore previous page.
                
                // We just set CurrentPage without pushing to history
                if (_currentPage != previousPage)
                {
                    _currentPage = previousPage;
                    PageChanged?.Invoke(this, previousPage);
                }
            }
        }

        public bool CanGoBack => _history.Count > 0;
    }
}
