/* GLOBALS */
let canvas, ctx;
let isDrawing = false;
let lastX = 0, lastY = 0;

let currentTool = "pen";
let currentColor = "#0A0908";

let NOTE_ID = 0;
let currentPage = 1;

let undoStack = [];
let redoStack = [];

/* INITIALIZE */
document.addEventListener("DOMContentLoaded", () =>
{

    const meta = document.getElementById("noteMeta");
    NOTE_ID = parseInt(meta.dataset.noteId);

    canvas = document.getElementById("noteCanvas");
    ctx = canvas.getContext("2d");

    setupCanvasSize();
    window.addEventListener("resize", setupCanvasSize);

    initToolbar();
    initCanvasEvents();
    initPages();
});

/* CANVAS SIZE */
function setupCanvasSize()
{
    const rect = canvas.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;

    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;

    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    ctx.lineJoin = "round";
    ctx.lineCap = "round";

    restoreImageForResize();
}

function restoreImageForResize()
{
    if (undoStack.length > 0) {
        restoreImage(undoStack[undoStack.length - 1]);
    }
}

/* TOOLBAR */
function initToolbar()
{
    document.getElementById("tool_pen").onclick = () => selectTool("pen");
    document.getElementById("tool_highlighter").onclick = () => selectTool("highlighter");
    document.getElementById("tool_eraser").onclick = () => selectTool("eraser");
    document.getElementById("tool_text").onclick = () => selectTool("text");

    selectTool("pen");
}

function selectTool(tool)
{
    currentTool = tool;

    document.querySelectorAll(".gn-tool-btn").forEach(btn =>
        btn.classList.remove("gn-tool-active")
    );

    document.getElementById("tool_" + tool).classList.add("gn-tool-active");
}

/* CANVAS EVENTS */
function initCanvasEvents()
{
    canvas.addEventListener("pointerdown", onPointerDown);
    canvas.addEventListener("pointermove", onPointerMove);
    canvas.addEventListener("pointerup", () => isDrawing = false);
    canvas.addEventListener("pointerleave", () => isDrawing = false);
}

function onPointerDown(e)
{
    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    if (currentTool === "text")
    {
        createTextBox(x, y);
        return;
    }

    isDrawing = true;
    lastX = x;
    lastY = y;

    saveState();
}

function onPointerMove(e)
{
    if (!isDrawing || currentTool === "text") return;

    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    const size = document.getElementById("brushSize").value;

    ctx.beginPath();
    ctx.moveTo(lastX, lastY);
    ctx.lineTo(x, y);

    if (currentTool === "pen")
    {
        ctx.globalCompositeOperation = "source-over";
        ctx.globalAlpha = 1.0;
        ctx.strokeStyle = currentColor;
        ctx.lineWidth = size;
    }

    if (currentTool === "highlighter")
    {
        ctx.globalCompositeOperation = "source-over";
        ctx.globalAlpha = 0.3;
        ctx.strokeStyle = currentColor;
        ctx.lineWidth = size * 2;
    }

    if (currentTool === "eraser")
    {
        ctx.globalCompositeOperation = "destination-out";
        ctx.globalAlpha = 1.0;
        ctx.lineWidth = size * 2;
    }

    ctx.stroke();
    lastX = x;
    lastY = y;
}

/* TEXT BOXES */
function createTextBox(x, y)
{
    const box = document.createElement("div");
    box.className = "gn-textbox";
    box.contentEditable = true;
    box.innerHTML = "Type…";

    box.style.left = x + "px";
    box.style.top = y + "px";

    enableDrag(box);

    const layer = document.getElementById("textLayer");
    layer.appendChild(box);

    layer.style.pointerEvents = "auto";
    box.focus();
}

function enableDrag(box)
{
    let isDown = false;
    let offsetX = 0, offsetY = 0;

    box.addEventListener("mousedown", (e) =>
    {
        isDown = true;
        offsetX = e.clientX - box.offsetLeft;
        offsetY = e.clientY - box.offsetTop;
        box.style.zIndex = 9999;
    });

    document.addEventListener("mouseup", () => isDown = false);

    document.addEventListener("mousemove", (e) =>
    {
        if (!isDown) return;
        box.style.left = (e.clientX - offsetX) + "px";
        box.style.top = (e.clientY - offsetY) + "px";
    });
}

/* PAGE HANDLING */
function initPages()
{
    const thumbs = document.querySelectorAll(".gn-thumb");
    if (thumbs.length > 0)
    {
        loadPage(parseInt(thumbs[0].dataset.page));
    }
}

function highlightPage()
{
    document.querySelectorAll(".gn-thumb").forEach(t =>
    {
        t.classList.remove("gn-thumb-active");
    });
    document.querySelector(`.gn-thumb[data-page='${currentPage}']`)
        ?.classList.add("gn-thumb-active");
}

function loadPage(pageNumber)
{

    currentPage = pageNumber;
    highlightPage();

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    document.getElementById("textLayer").innerHTML = "";

    const thumb = document.querySelector(`.gn-thumb[data-page="${pageNumber}"]`);

    if (thumb && thumb.dataset.image && thumb.dataset.image !== "")
    {
        restoreImage(thumb.dataset.image);
    }

    undoStack = [];
    redoStack = [];
    saveState();
}

/* Restore image */
function restoreImage(dataUrl)
{
    const img = new Image();
    img.onload = () =>
    {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
    };
    img.src = dataUrl;
}

/*  ADD PAGE */
function addNewPage()
{
    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID);

    fetch("/Notes/AddPage",
        {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    })
        .then(r => r.json())
        .then(data =>
        {
            const num = data.pageNumber;

            const div = document.createElement("div");
            div.className = "gn-thumb";
            div.dataset.page = num;
            div.dataset.image = "";
            div.onclick = () => loadPage(num);

            const span = document.createElement("span");
            span.textContent = "Page " + num;

            div.appendChild(span);
            document.getElementById("pageList").appendChild(div);

            loadPage(num);
        });
}

/* DELETE PAGE */
function deleteCurrentPage()
{
    if (!confirm("Delete this page?")) return;

    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID);
    formData.append("pageNumber", currentPage);

    fetch("/Notes/DeletePage",
        {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    })
        .then(() =>
        {
            document.querySelector(`.gn-thumb[data-page="${currentPage}"]`)?.remove();

            const thumbs = document.querySelectorAll(".gn-thumb");

            if (thumbs.length > 0) {
                loadPage(parseInt(thumbs[0].dataset.page));
            } else {
                addNewPage();
            }
        });
}

/* SAVE PAGE */
function saveCurrentPage()
{

    const imageData = canvas.toDataURL("image/png");

    let textData = [];
    document.querySelectorAll(".gn-textbox").forEach(b =>
    {
        textData.push({
            x: parseInt(b.style.left),
            y: parseInt(b.style.top),
            width: b.offsetWidth,
            height: b.offsetHeight,
            html: b.innerHTML
        });
    });

    const formData = new URLSearchParams();
    formData.append("noteId", NOTE_ID);
    formData.append("pageNumber", currentPage);
    formData.append("imageData", imageData);
    formData.append("textBoxes", JSON.stringify(textData));

    fetch("/Notes/SavePage",
        {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: formData.toString()
    })
        .then(() =>
        {
            const thumb = document.querySelector(`.gn-thumb[data-page="${currentPage}"]`);
            if (thumb) thumb.dataset.image = imageData;

            const btn = document.querySelector(".gn-save-btn");
            btn.textContent = "Saved ✓";
            setTimeout(() => btn.textContent = "Save Page", 900);
        });
}

/* UNDO / REDO */
function saveState()
{
    undoStack.push(canvas.toDataURL());
    if (undoStack.length > 50) undoStack.shift();
}

function undoCanvas()
{
    if (undoStack.length <= 1) return;

    redoStack.push(undoStack.pop());
    restoreImage(undoStack[undoStack.length - 1]);
}

function redoCanvas()
{
    if (redoStack.length === 0) return;

    const img = redoStack.pop();
    undoStack.push(img);
    restoreImage(img);
}
