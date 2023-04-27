mergeInto(LibraryManager.library, {

  BrowserApplicationStarted: function () {
    window.FromUnity_ApplicationStarted();
  },

  BrowserSelect: function (str) {
    window.FromUnity_Select(UTF8ToString(str));
  },
  
  BrowserItemListString: function (str) {
    window.FromUnity_SetListItems(UTF8ToString(str));
  }
});