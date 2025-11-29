let canvas, ctx;
let isDrawing = false;
let lastX = 0;
let lastY = 0;
let currentTool = 'pen';        // 'pen' | 'highlighter' | 'eraser'
let currentColor = '#0A0908';
let currentPage = 1;
let undoStack = [];
let redoStack = [];

document.addEventListener('DOMContentLoaded', () => {
    canvas = document.getElementById('noteCanvas');
    ctx = canvas.getContext('2d');

    setupCanvasSize();
    initToolbarDefaults();
    initCanvasEvents();
    initPages();
});

/* --------- Setup & Resizing ---------------------------------------- */

function setupCanvasSize() {
    const rect = canvas.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;

    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;

    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    ctx.lineJoin = 'round';
    ctx.lineCap = 'round';

    // White background
    ctx.fillStyle = '#FFFFFF';
    ctx.fillRect(0, 0, rect.width, rect.height);
}

window.addEventListener('resize', () => {
    // On resize, we could re-calc, but to keep it simple we ignore for now
});

/* --------- Toolbar logic -------------------------------------------- */

function initToolbarDefaults() {
    // Default tool = pen
    selectTool('pen');
    setColor('#0A0908');

    const sizeSlider = document.getElementById('brushSize');
    if (sizeSlider) {
        sizeSlider.addEventListener('input', () => {
            // nothing needed here; we read value in drawing
        });
    }
}

function selectTool(tool) {
    currentTool = tool;

    // toggle active styles on buttons
    document.querySelectorAll('.gn-tool-btn').forEach(btn => {
        btn.classList.remove('gn-tool-active');
    });

    const activeId = 'tool_' + tool;
    const activeBtn = document.getElementById(activeId);
    if (activeBtn) {
        activeBtn.classList.add('gn-tool-active');
    }
}

function setColor(color) {
    currentColor = color;
}

/* --------- Canvas Draw Events --------------------------------------- */

function initCanvasEvents() {
    canvas.addEventListener('pointerdown', onPointerDown);
    canvas.addEventListener('pointermove', onPointerMove);
    canvas.addEventListener('pointerup', onPointerUp);
    canvas.addEventListener('pointerleave', onPointerUp);

    canvas.style.touchAction = 'none';
}

function getBrushSize() {
    const slider = document.getElementById('brushSize');
    return slider ? parseInt(slider.value || '6', 10) : 6;
}

function onPointerDown(e) {
    e.preventDefault();

    saveCanvasState();  // snapshot for undo

    isDrawing = true;

    const rect = canvas.getBoundingClientRect();
    lastX = e.clientX - rect.left;
    lastY = e.clientY - rect.top;
}

function onPointerMove(e) {
    if (!isDrawing) return;
    e.preventDefault();

    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    ctx.beginPath();
    ctx.moveTo(lastX, lastY);
    ctx.lineTo(x, y);

    const size = getBrushSize();

    if (currentTool === 'pen') {
        ctx.globalCompositeOperation = 'source-over';
        ctx.globalAlpha = 1.0;
        ctx.strokeStyle = currentColor;
        ctx.lineWidth = size;
    }
    else if (currentTool === 'highlighter') {
        ctx.globalCompositeOperation = 'source-over';
        ctx.globalAlpha = 0.3;
        ctx.strokeStyle = currentColor;
        ctx.lineWidth = size * 1.8;
    }
    else if (currentTool === 'eraser') {
        ctx.globalCompositeOperation = 'destination-out';
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

/* --------- Undo / Redo ---------------------------------------------- */

function saveCanvasState() {
    try {
        const dataUrl = canvas.toDataURL('image/png');
        undoStack.push(dataUrl);

        if (undoStack.length > 30) {
            undoStack.shift();
        }

        // when we draw a new stroke, clear redo history
        redoStack = [];
    } catch (err) {
        console.error('Unable to save canvas state:', err);
    }
}

function restoreFromDataUrl(dataUrl) {
    const img = new Image();
    img.onload = () => {
        const rect = canvas.getBoundingClientRect();
        ctx.clearRect(0, 0, rect.width, rect.height);
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(0, 0, rect.width, rect.height);
        ctx.drawImage(img, 0, 0, rect.width, rect.height);
    };
    img.src = dataUrl;
}

function undoCanvas() {
    if (undoStack.length === 0) return;

    // current state goes to redo
    const current = canvas.toDataURL('image/png');
    redoStack.push(current);

    const previous = undoStack.pop();
    if (previous) {
        restoreFromDataUrl(previous);
    }
}

function redoCanvas() {
    if (redoStack.length === 0) return;

    const current = canvas.toDataURL('image/png');
    undoStack.push(current);

    const state = redoStack.pop();
    if (state) {
        restoreFromDataUrl(state);
    }
}

/* --------- Pages / Thumbnails ---------------------------------------- */

function initPages() {
    const thumbs = document.querySelectorAll('.gn-thumb');
    if (thumbs.length > 0) {
        // load first page
        const first = thumbs[0];
        const pageNum = parseInt(first.dataset.page || '1', 10);
        loadPage(pageNum);
    } else {
        // no pages, create one
        addNewPage();
    }
}

function highlightActiveThumb() {
    document.querySelectorAll('.gn-thumb').forEach(el => {
        el.classList.remove('gn-thumb-active');
        const p = parseInt(el.dataset.page || '0', 10);
        if (p === currentPage) {
            el.classList.add('gn-thumb-active');
        }
    });
}

function loadPage(pageNumber) {
    currentPage = pageNumber;

    const thumb = document.querySelector(`.gn-thumb[data-page="${pageNumber}"]`);
    const rect = canvas.getBoundingClientRect();

    // wipe canvas
    ctx.clearRect(0, 0, rect.width, rect.height);
    ctx.fillStyle = '#FFFFFF';
    ctx.fillRect(0, 0, rect.width, rect.height);

    if (thumb) {
        const imgData = thumb.dataset.image;
        if (imgData && imgData.startsWith('data:image')) {
            restoreFromDataUrl(imgData);
        }
    }

    highlightActiveThumb();
    undoStack = [];
    redoStack = [];
    saveCanvasState(); // initial state of this page
}

function addNewPage() {
    const formData = new URLSearchParams();
    formData.append('noteId', NOTE_ID);

    fetch('/Notes/AddPage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(r => r.json())
        .then(data => {
            const newPage = data.pageNumber;

            const pageList = document.getElementById('pageList');
            const div = document.createElement('div');
            div.className = 'gn-thumb';
            div.dataset.page = newPage;
            div.dataset.image = '';
            div.onclick = () => loadPage(newPage);

            const span = document.createElement('span');
            span.textContent = `Page ${newPage}`;
            div.appendChild(span);

            pageList.appendChild(div);

            loadPage(newPage);
        })
        .catch(err => console.error('Error adding page:', err));
}

/* --------- Save Current Page ----------------------------------------- */

function saveCurrentPage() {
    const dataUrl = canvas.toDataURL('image/png');

    const formData = new URLSearchParams();
    formData.append('noteId', NOTE_ID);
    formData.append('pageNumber', currentPage);
    formData.append('imageData', dataUrl);

    fetch('/Notes/SavePage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(r => {
            if (!r.ok) throw new Error('Save failed');
            return r.text();
        })
        .then(() => {
            // update thumbnail's data-image
            const thumb = document.querySelector(`.gn-thumb[data-page="${currentPage}"]`);
            if (thumb) {
                thumb.dataset.image = dataUrl;
            }

            // small visual feedback
            const saveBtn = document.querySelector('.gn-save-btn');
            if (saveBtn) {
                saveBtn.textContent = 'Saved ✓';
                setTimeout(() => saveBtn.textContent = 'Save Page', 1200);
            }
        })
        .catch(err => console.error('Error saving page:', err));
}

/* --------- (Optional) Delete Page ------------------------------------ */

function deleteCurrentPage() {
    const formData = new URLSearchParams();
    formData.append('noteId', NOTE_ID);
    formData.append('pageNumber', currentPage);

    fetch('/Notes/DeletePage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(r => {
            if (!r.ok) throw new Error('Delete failed');
            return r.text();
        })
        .then(() => {
            const thumb = document.querySelector(`.gn-thumb[data-page="${currentPage}"]`);
            if (thumb && thumb.parentElement) {
                thumb.parentElement.removeChild(thumb);
            }

            const thumbs = document.querySelectorAll('.gn-thumb');
            if (thumbs.length > 0) {
                const first = thumbs[0];
                const p = parseInt(first.dataset.page || '1', 10);
                loadPage(p);
            } else {
                addNewPage();
            }
        })
        .catch(err => console.error('Error deleting page:', err));
}
