let canvas, ctx;
let isDrawing = false;
let lastX = 0, lastY = 0;
let currentTool = "pen";      // "pen" | "highlighter" | "eraser" | "text"
let currentColor = "#0A0908";
let currentPage = 1;
let undoStack = [];
let redoStack = [];
let NOTE_ID = 0;
let PAGES_COUNT = 0;

document.addEventListener("DOMContentLoaded", () => {
    const meta = document.getElementById("noteMeta");
    if (meta) {
        NOTE_ID = parseInt(meta.dataset.noteId || "0");
        PAGES_COUNT = parseInt(meta.dataset.pagesCount || "0");
    }

    canvas = document.getElementById("noteCanvas");
    if (!canvas) return;

    ctx = canvas.getContext("2d");

    setupCanvasSize();
    window.addEventListener("resize", setupCanvasSize);

    initToolbarDefaults();
    initCanvasEvents();
    initPages();
});

/* ---------- Canvas size --------------------------------------------- */

function setupCanvasSize() {
    const rect = canvas.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;

    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;

    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    ctx.lineJoin = "round";
    ctx.lineCap = "round";

    ctx.clearRect(0, 0, rect.width, rect.height);
}

/* ---------- Toolbar / tools ----------------------------------------- */

function initToolbarDefaults() {
    selectTool("pen");
    setColor("#0A0908");
}

function selectTool(tool) {
    currentTool = tool;

    document.querySelectorAll(".gn-tool-btn").forEach(btn => {
        btn.classList.remove("gn-tool-active");
    });

    const active = document.getElementById("tool_" + tool);
    if (active) active.classList.add("gn-tool-active");
}

function setColor(color) {
    currentColor = color;
}

function getBrushSize() {
    const slider = document.getElementById("brushSize");
    return slider ? parseInt(slider.value || "12", 10) : 12;
}

/* ---------- Drawing events ------------------------------------------ */

function initCanvasEvents() {
    canvas.addEventListener("pointerdown", onPointerDown);
    canvas.addEventListener("pointermove", onPointerMove);
    canvas.addEventListener("pointerup", onPointerUp);
    canvas.addEventListener("pointerleave", onPointerUp);

    canvas.style.touchAction = "none";
}

function onPointerDown(e) {
    if (currentTool === "text") {
        // text mode: focus textarea instead of drawing
        const textArea = document.getElementById("noteText");
        if (textArea) textArea.focus();
        return;
    }

    e.preventDefault();
    saveCanvasState();

    isDrawing = true;

    const rect = canvas.getBoundingClientRect();
    lastX = e.clientX - rect.left;
    lastY = e.clientY - rect.top;
}

function onPointerMove(e) {
    if (!isDrawing || currentTool === "text") return;

    e.preventDefault();

    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    ctx.beginPath();
    ctx.moveTo(lastX, lastY);
    ctx.lineTo(x, y);

    const size = getBrushSize();

    if (currentTool === "pen") {
        ctx.globalCompositeOperation = "source-over";
        ctx.globalAlpha = 1.0;
        ctx.strokeStyle = currentColor;
        ctx.lineWidth = size;
    } else if (currentTool === "highlighter") {
        ctx.globalCompositeOperation = "source-over";
        ctx.globalAlpha = 0.3;
        ctx.strokeStyle = currentColor;
        ctx.lineWidth = size * 1.8;
    } else if (currentTool === "eraser") {
        ctx.globalCompositeOperation = "destination-out";
        ctx.globalAlpha = 1.0;
        ctx.lineWidth = size * 2;
    }

    ctx.stroke();
    ctx.closePath();

    lastX = x;
    lastY = y;
}

function onPointerUp(e) {
    if (!isDrawing) return;
    e.preventDefault();
    isDrawing = false;
}

/* ---------- Undo / redo --------------------------------------------- */

function saveCanvasState() {
    try {
        const dataUrl = canvas.toDataURL("image/png");
        undoStack.push(dataUrl);

        if (undoStack.length > 40) undoStack.shift();
        redoStack = [];
    } catch (err) {
        console.error("Unable to save canvas state:", err);
    }
}

function restoreFromDataUrl(dataUrl) {
    const img = new Image();
    img.onload = () => {
        const rect = canvas.getBoundingClientRect();
        ctx.clearRect(0, 0, rect.width, rect.height);
        ctx.drawImage(img, 0, 0, rect.width, rect.height);
    };
    img.src = dataUrl;
}

function undoCanvas() {
    if (undoStack.length === 0) return;

    const current = canvas.toDataURL("image/png");
    redoStack.push(current);

    const previous = undoStack.pop();
    if (previous) restoreFromDataUrl(previous);
}

function redoCanvas() {
    if (redoStack.length === 0) return;

    const current = canvas.toDataURL("image/png");
    undoStack.push(current);

    const state = redoStack.pop();
    if (state) restoreFromDataUrl(state);
}

/* ---------- Pages ---------------------------------------------------- */

function initPages() {
    const thumbs = document.querySelectorAll(".gn-thumb");
    if (thumbs.length > 0) {
        const first = thumbs[0];
        const pageNum = parseInt(first.dataset.page || "1", 10);
        loadPage(pageNum);
    } else {
        addNewPage();
    }
}

function highlightActiveThumb() {
    document.querySelectorAll(".gn-thumb").forEach(el => {
        el.classList.remove("gn-thumb-active");
        const p = parseInt(el.dataset.page || "0", 10);
        if (p === currentPage) {
            el.classList.add("gn-thumb-active");
        }
    });
}

function loadPage(pageNumber) {
    currentPage = pageNumber;

    const rect = canvas.getBoundingClientRect();
    ctx.clearRect(0, 0, rect.width, rect.height);

    const thumb = document.querySelector(`.gn-thumb[data-page="${pageNumber}"]`);
    if (thumb) {
        const imgData = thumb.dataset.image;
        if (imgData && imgData.startsWith("data:image")) {
            restoreFromDataUrl(imgData);
        }
    }

    highlightActiveThumb();
    undoStack = [];
    redoStack = [];
    saveCanvasState();
}

function addNewPage() {
    if (!NOTE_ID) return;

    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID.toString());

    fetch("/Notes/AddPage", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    })
        .then(r => r.json())
        .then(data => {
            const newPage = data.pageNumber;

            const pageList = document.getElementById("pageList");
            const div = document.createElement("div");
            div.className = "gn-thumb";
            div.dataset.page = newPage;
            div.dataset.image = "";
            div.onclick = () => loadPage(newPage);

            const span = document.createElement("span");
            span.textContent = `Page ${newPage}`;
            div.appendChild(span);

            pageList.appendChild(div);

            loadPage(newPage);
        })
        .catch(err => console.error("Error adding page:", err));
}

/* ---------- Save text + page ---------------------------------------- */

function saveNoteText() {
    if (!NOTE_ID) return;

    const textArea = document.getElementById("noteText");
    if (!textArea) return;

    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID.toString());
    formData.append("content", textArea.value);

    return fetch("/Notes/SaveText", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    }).then(r => {
        if (!r.ok) throw new Error("Save text failed");
    }).catch(err => console.error(err));
}

function saveCurrentPage() {
    if (!NOTE_ID) return;

    // save typed text
    saveNoteText();

    // save drawing as image
    const dataUrl = canvas.toDataURL("image/png");

    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID.toString());
    formData.append("pageNumber", currentPage.toString());
    formData.append("imageData", dataUrl);

    fetch("/Notes/SavePage", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    })
        .then(r => {
            if (!r.ok) throw new Error("Save failed");
            return r.text();
        })
        .then(() => {
            const thumb = document.querySelector(`.gn-thumb[data-page="${currentPage}"]`);
            if (thumb) thumb.dataset.image = dataUrl;

            const saveBtn = document.querySelector(".gn-save-btn");
            if (saveBtn) {
                saveBtn.textContent = "Saved ✓";
                setTimeout(() => saveBtn.textContent = "Save Page", 1200);
            }
        })
        .catch(err => console.error("Error saving page:", err));
}

/* ---------- Optional delete (not wired in UI yet) ------------------- */

function deleteCurrentPage() {
    if (!NOTE_ID) return;

    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID.toString());
    formData.append("pageNumber", currentPage.toString());

    fetch("/Notes/DeletePage", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    })
        .then(r => {
            if (!r.ok) throw new Error("Delete failed");
            return r.text();
        })
        .then(() => {
            const thumb = document.querySelector(`.gn-thumb[data-page="${currentPage}"]`);
            if (thumb && thumb.parentElement) {
                thumb.parentElement.removeChild(thumb);
            }

            const thumbs = document.querySelectorAll(".gn-thumb");
            if (thumbs.length > 0) {
                const first = thumbs[0];
                const p = parseInt(first.dataset.page || "1", 10);
                loadPage(p);
            } else {
                addNewPage();
            }
        })
        .catch(err => console.error("Error deleting page:", err));
}
