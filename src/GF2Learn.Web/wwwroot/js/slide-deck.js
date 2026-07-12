(function (global) {
  var deckEl = null;
  var dotNetRef = null;
  var keyHandler = null;

  function isEditableTarget(target) {
    if (!target) return false;
    if (target.isContentEditable) return true;
    var tag = target.tagName;
    if (tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT") return true;
    if (target.closest(".monaco-editor")) return true;
    return false;
  }

  function wireKeyboard() {
    if (keyHandler) return;

    keyHandler = function (e) {
      if (!dotNetRef) return;
      if (isEditableTarget(e.target)) return;

      switch (e.key) {
        case "ArrowRight":
        case "PageDown":
          e.preventDefault();
          dotNetRef.invokeMethodAsync("NextSlide");
          break;
        case " ":
          if (e.shiftKey) {
            e.preventDefault();
            dotNetRef.invokeMethodAsync("PreviousSlide");
          } else {
            e.preventDefault();
            dotNetRef.invokeMethodAsync("NextSlide");
          }
          break;
        case "ArrowLeft":
        case "PageUp":
          e.preventDefault();
          dotNetRef.invokeMethodAsync("PreviousSlide");
          break;
        case "Home":
          e.preventDefault();
          dotNetRef.invokeMethodAsync("GoToSlide", 0);
          break;
        case "End":
          e.preventDefault();
          dotNetRef.invokeMethodAsync("GoToLastSlide");
          break;
        case "g":
        case "G":
          e.preventDefault();
          dotNetRef.invokeMethodAsync("ToggleOverviewFromJs");
          break;
        case "f":
        case "F":
          e.preventDefault();
          toggleFullscreen(deckEl);
          break;
        case "Escape":
          if (global.document.fullscreenElement) {
            e.preventDefault();
            global.document.exitFullscreen();
          }
          break;
        default:
          break;
      }
    };

    global.document.addEventListener("keydown", keyHandler);
  }

  function unwireKeyboard() {
    if (!keyHandler) return;
    global.document.removeEventListener("keydown", keyHandler);
    keyHandler = null;
  }

  function onSlideChanged(slideElement) {
    if (!slideElement) return;

    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        if (global.gf2Theme) global.gf2Theme.apply(global.gf2Theme.getMode());
        if (global.gf2Highlight) global.gf2Highlight.process(slideElement);
        if (global.gf2KnowledgeCheck) global.gf2KnowledgeCheck.init(slideElement);
        if (global.gf2Playground && global.gf2Playground.ensureMountedWithRetries) {
          global.gf2Playground.ensureMountedWithRetries();
        } else if (global.gf2Playground && global.gf2Playground.ensureMounted) {
          global.gf2Playground.ensureMounted();
        }
        if (global.gf2Playground && global.gf2Playground.refreshAll) {
          global.gf2Playground.refreshAll();
        }
      });
    });
  }

  function toggleFullscreen(element) {
    if (!element) return;

    if (!global.document.fullscreenElement) {
      element.requestFullscreen().catch(function () {});
      return;
    }

    global.document.exitFullscreen().catch(function () {});
  }

  function init(element, interopRef) {
    deckEl = element;
    dotNetRef = interopRef;
    wireKeyboard();
  }

  function dispose() {
    unwireKeyboard();
    dotNetRef = null;
    deckEl = null;
  }

  global.gf2SlideDeck = {
    init: init,
    dispose: dispose,
    onSlideChanged: onSlideChanged,
    toggleFullscreen: toggleFullscreen
  };
})(window);
