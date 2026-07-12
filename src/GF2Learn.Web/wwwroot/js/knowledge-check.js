(function (global) {
  function apiUrl(path) {
    var base = document.querySelector("base");
    var href = base ? base.getAttribute("href") || "/" : "/";
    if (!href.endsWith("/")) href += "/";
    return href + path.replace(/^\//, "");
  }

  function normalizeAnswer(answer) {
    return {
      questionIndex: answer.questionIndex ?? answer.QuestionIndex,
      selectedIndex: answer.selectedIndex ?? answer.SelectedIndex,
      isCorrect: answer.isCorrect ?? answer.IsCorrect
    };
  }

  function readBootstrapAnswers(contentSlug) {
    var el = document.getElementById("kc-bootstrap-" + contentSlug);
    if (!el || !el.textContent) return null;

    try {
      var parsed = JSON.parse(el.textContent);
      return Array.isArray(parsed) ? parsed : null;
    } catch (e) {
      console.warn("Knowledge check: kunne ikke læse gemte svar fra siden.", e);
      return null;
    }
  }

  function shuffleOptions(question) {
    if (question.dataset.kcShuffled === "1") return;
    question.dataset.kcShuffled = "1";

    var list = question.querySelector(".kc-options");
    if (!list) return;

    var items = Array.from(list.children);
    for (var i = items.length - 1; i > 0; i--) {
      var j = Math.floor(Math.random() * (i + 1));
      var temp = items[i];
      items[i] = items[j];
      items[j] = temp;
    }

    items.forEach(function (item) {
      list.appendChild(item);
    });
  }

  function applyAnswer(question, selectedOriginalIndex, isCorrect) {
    var correctOriginal = parseInt(question.dataset.correct, 10);
    var feedback = question.querySelector(".kc-feedback");
    var verdict = question.querySelector(".kc-verdict");
    var buttons = question.querySelectorAll(".kc-option");

    question.classList.add("kc-answered");

    buttons.forEach(function (option) {
      var originalIndex = parseInt(option.dataset.originalIndex, 10);
      option.disabled = true;
      if (originalIndex === correctOriginal) option.classList.add("kc-correct");
      else if (originalIndex === selectedOriginalIndex && !isCorrect) option.classList.add("kc-wrong");
    });

    verdict.textContent = isCorrect
      ? "Korrekt! Her er hvorfor:"
      : "Ikke helt. Det rigtige svar er markeret — her er forklaringen:";
    verdict.className = "kc-verdict " + (isCorrect ? "kc-verdict-ok" : "kc-verdict-no");
    feedback.hidden = false;
  }

  function markChapterCompleteInNav(contentSlug) {
    if (!contentSlug) return;

    document.querySelectorAll('[data-curriculum-slug="' + contentSlug + '"]').forEach(function (link) {
      if (link.querySelector(".sidebar-check, .index-check")) return;

      if (link.classList.contains("sidebar-link")) {
        var check = document.createElement("span");
        check.className = "sidebar-check";
        check.setAttribute("aria-label", "Fuldført");
        check.setAttribute("title", "Alle spørgsmål besvaret");
        check.textContent = "✓";
        link.appendChild(check);
        return;
      }

      if (link.classList.contains("index-item")) {
        var arrow = link.querySelector(".index-arrow");
        if (arrow) arrow.remove();
        var indexCheck = document.createElement("span");
        indexCheck.className = "index-check";
        indexCheck.setAttribute("aria-label", "Fuldført");
        indexCheck.setAttribute("title", "Alle spørgsmål besvaret");
        indexCheck.textContent = "✓";
        link.appendChild(indexCheck);
      }
    });
  }

  function updateProgressLabel(section) {
    var progressEl = section.querySelector(".kc-progress");
    if (!progressEl) return;

    var total = section.querySelectorAll(".kc-question").length;
    var answered = section.querySelectorAll(".kc-question.kc-answered").length;
    if (total === 0) {
      progressEl.hidden = true;
      return;
    }

    progressEl.textContent = answered + " af " + total + " besvaret";
    progressEl.hidden = false;

    if (answered === total) {
      maybeMarkChapterComplete(section);
    }
  }

  function maybeMarkChapterComplete(section) {
    var contentSlug = section.dataset.contentSlug;
    if (!contentSlug) return;

    if (section.classList.contains("knowledge-check--slide")) {
      var totalQuestions = parseInt(section.dataset.totalQuestions, 10);
      if (!totalQuestions) return;

      loadAnswers(contentSlug).then(function (answers) {
        if (answers.length >= totalQuestions) {
          markChapterCompleteInNav(contentSlug);
        }
      });
      return;
    }

    markChapterCompleteInNav(contentSlug);
  }

  function saveAnswer(contentSlug, questionIndex, selectedOriginalIndex, isCorrect) {
    return fetch(apiUrl("/api/progress/knowledge-check"), {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        contentSlug: contentSlug,
        questionIndex: questionIndex,
        selectedIndex: selectedOriginalIndex,
        isCorrect: isCorrect
      })
    });
  }

  function loadAnswers(contentSlug) {
    return fetch(apiUrl("/api/progress/knowledge-check/" + encodeURIComponent(contentSlug)), {
      credentials: "include",
      headers: { Accept: "application/json" }
    }).then(function (response) {
      if (response.status === 401) return [];
      if (!response.ok) return [];

      var contentType = response.headers.get("content-type") || "";
      if (contentType.indexOf("application/json") === -1) return [];

      return response.json().then(function (data) {
        return Array.isArray(data) ? data : [];
      });
    }).catch(function () {
      return [];
    });
  }

  function wireQuestion(question, section) {
    if (question.dataset.kcWired === "1") return;
    question.dataset.kcWired = "1";

    var contentSlug = section.dataset.contentSlug;
    var questionIndex = parseInt(question.dataset.questionIndex, 10);
    var correctOriginal = parseInt(question.dataset.correct, 10);
    var buttons = question.querySelectorAll(".kc-option");

    buttons.forEach(function (button) {
      button.addEventListener("click", function () {
        if (question.classList.contains("kc-answered")) return;

        var selectedOriginal = parseInt(button.dataset.originalIndex, 10);
        var isCorrect = selectedOriginal === correctOriginal;
        applyAnswer(question, selectedOriginal, isCorrect);
        updateProgressLabel(section);

        if (!contentSlug) return;

        saveAnswer(contentSlug, questionIndex, selectedOriginal, isCorrect).then(function (response) {
          if (response.ok) {
            maybeMarkChapterComplete(section);
            return;
          }

          if (response.status === 401 && section.dataset.kcLoginHintShown !== "1") {
            section.dataset.kcLoginHintShown = "1";
            var hint = document.createElement("p");
            hint.className = "kc-login-hint";
            hint.textContent = "Log ind for at gemme din progress på tværs af enheder.";
            section.querySelector(".kc-heading").after(hint);
            return;
          }

          if (section.dataset.kcSaveErrorShown !== "1") {
            section.dataset.kcSaveErrorShown = "1";
            var errorHint = document.createElement("p");
            errorHint.className = "kc-login-hint kc-save-error";
            errorHint.textContent = "Kunne ikke gemme dit svar lige nu. Prøv igen om lidt.";
            section.querySelector(".kc-heading").after(errorHint);
          }
        });
      });
    });
  }

  function restoreAnswers(section, answers) {
    if (!answers || !answers.length) return;

    answers.forEach(function (raw) {
      var answer = normalizeAnswer(raw);
      if (answer.questionIndex === undefined || answer.selectedIndex === undefined) return;

      var question = section.querySelector('.kc-question[data-question-index="' + answer.questionIndex + '"]');
      if (!question || question.classList.contains("kc-answered")) return;
      applyAnswer(question, answer.selectedIndex, !!answer.isCorrect);
    });

    updateProgressLabel(section);
  }

  function initSection(section) {
    var contentSlug = section.dataset.contentSlug;
    if (!contentSlug) return;

    section.querySelectorAll(".kc-question").forEach(shuffleOptions);

    function finishInit(answers) {
      restoreAnswers(section, answers);
      section.querySelectorAll(".kc-question").forEach(function (question) {
        wireQuestion(question, section);
      });
    }

    var bootstrap = readBootstrapAnswers(contentSlug);
    if (bootstrap) {
      finishInit(bootstrap);
      return;
    }

    loadAnswers(contentSlug).then(finishInit);
  }

  function initKnowledgeCheck(root) {
    root = root || document;
    root.querySelectorAll(".knowledge-check").forEach(initSection);
  }

  function scheduleInit(root) {
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        initKnowledgeCheck(root);
      });
    });
  }

  function wire() {
    scheduleInit(document);
    global.addEventListener("gf2-enhanced-nav", function () {
      scheduleInit(document);
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", wire);
  } else {
    wire();
  }

  global.gf2KnowledgeCheck = { init: initKnowledgeCheck };
})(window);
