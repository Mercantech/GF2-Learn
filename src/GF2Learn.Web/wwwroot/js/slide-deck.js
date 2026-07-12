(function (global) {
  var activeDeck = null;

  function isEditableTarget(target) {
    if (!target) return false;
    if (target.isContentEditable) return true;
    var tag = target.tagName;
    if (tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT") return true;
    if (target.closest(".monaco-editor")) return true;
    return false;
  }

  function enhanceSlide(slideElement) {
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

  function createDeckController(deck) {
    var slides = Array.from(deck.querySelectorAll(".slide-deck-slide-panel"));
    var dots = Array.from(deck.querySelectorAll(".slide-deck-dot"));
    var overviewItems = Array.from(deck.querySelectorAll(".slide-deck-overview-item"));
    var overview = deck.querySelector(".slide-deck-overview");
    var overviewBtn = deck.querySelector('[data-slide-action="overview"]');
    var prevBtn = deck.querySelector('[data-slide-action="prev"]');
    var nextBtn = deck.querySelector('[data-slide-action="next"]');
    var counterCurrent = deck.querySelector(".slide-deck-counter-current");
    var currentIndex = 0;
    var overviewOpen = false;

    function setOverview(open) {
      overviewOpen = open;
      deck.classList.toggle("slide-deck--overview-open", overviewOpen);
      if (overview) overview.hidden = !overviewOpen;
      if (overviewBtn) overviewBtn.setAttribute("aria-pressed", overviewOpen ? "true" : "false");
    }

    function updateUi() {
      slides.forEach(function (slide, index) {
        var active = index === currentIndex;
        slide.classList.toggle("slide-deck-slide-panel--active", active);
        slide.hidden = !active;
      });

      dots.forEach(function (dot, index) {
        var active = index === currentIndex;
        dot.classList.toggle("slide-deck-dot--active", active);
        dot.setAttribute("aria-current", active ? "step" : "false");
      });

      overviewItems.forEach(function (item, index) {
        item.classList.toggle("slide-deck-overview-item--active", index === currentIndex);
      });

      if (prevBtn) prevBtn.disabled = currentIndex <= 0;
      if (nextBtn) nextBtn.disabled = currentIndex >= slides.length - 1;
      if (counterCurrent) counterCurrent.textContent = String(currentIndex + 1);
    }

    function showSlide(index) {
      if (index < 0 || index >= slides.length) return;
      currentIndex = index;
      updateUi();
      enhanceSlide(slides[currentIndex]);
    }

    function nextSlide() {
      showSlide(currentIndex + 1);
    }

    function previousSlide() {
      showSlide(currentIndex - 1);
    }

    function onClick(e) {
      var actionEl = e.target.closest("[data-slide-action]");
      if (actionEl && deck.contains(actionEl)) {
        var action = actionEl.getAttribute("data-slide-action");
        if (action === "prev") {
          e.preventDefault();
          previousSlide();
          return;
        }
        if (action === "next") {
          e.preventDefault();
          nextSlide();
          return;
        }
        if (action === "overview") {
          e.preventDefault();
          setOverview(!overviewOpen);
          return;
        }
        if (action === "fullscreen") {
          e.preventDefault();
          toggleFullscreen(deck);
          return;
        }
      }

      var gotoEl = e.target.closest("[data-slide-goto]");
      if (gotoEl && deck.contains(gotoEl)) {
        e.preventDefault();
        var index = parseInt(gotoEl.getAttribute("data-slide-goto"), 10);
        if (!isNaN(index)) {
          setOverview(false);
          showSlide(index);
        }
      }
    }

    function onKeyDown(e) {
      if (!deck.isConnected) return;
      if (isEditableTarget(e.target)) return;

      switch (e.key) {
        case "ArrowRight":
        case "PageDown":
          e.preventDefault();
          nextSlide();
          break;
        case " ":
          e.preventDefault();
          if (e.shiftKey) previousSlide();
          else nextSlide();
          break;
        case "ArrowLeft":
        case "PageUp":
          e.preventDefault();
          previousSlide();
          break;
        case "Home":
          e.preventDefault();
          showSlide(0);
          break;
        case "End":
          e.preventDefault();
          showSlide(slides.length - 1);
          break;
        case "g":
        case "G":
          e.preventDefault();
          setOverview(!overviewOpen);
          break;
        case "f":
        case "F":
          e.preventDefault();
          toggleFullscreen(deck);
          break;
        case "Escape":
          if (overviewOpen) {
            e.preventDefault();
            setOverview(false);
          } else if (global.document.fullscreenElement) {
            e.preventDefault();
            global.document.exitFullscreen();
          }
          break;
        default:
          break;
      }
    }

    deck.addEventListener("click", onClick);
    global.document.addEventListener("keydown", onKeyDown);

    updateUi();
    enhanceSlide(slides[0]);

    return {
      dispose: function () {
        deck.removeEventListener("click", onClick);
        global.document.removeEventListener("keydown", onKeyDown);
      }
    };
  }

  function initDeck(deck) {
    if (!deck || deck.dataset.slideDeckReady === "1") return;
    deck.dataset.slideDeckReady = "1";
    activeDeck = createDeckController(deck);
  }

  function initAll(root) {
    root = root || global.document;
    root.querySelectorAll("[data-slide-deck]").forEach(initDeck);
  }

  function wire() {
    initAll(global.document);
    global.addEventListener("gf2-enhanced-nav", function () {
      initAll(global.document);
    });
  }

  if (global.document.readyState === "loading") {
    global.document.addEventListener("DOMContentLoaded", wire);
  } else {
    wire();
  }

  global.gf2SlideDeck = {
    initAll: initAll,
    toggleFullscreen: toggleFullscreen
  };
})(window);
