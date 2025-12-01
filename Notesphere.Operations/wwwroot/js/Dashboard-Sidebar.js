let locked = false;

const sidebar = document.getElementById("notionSidebar");
const app = document.getElementById("appWrapper");
const lockBtn = document.getElementById("lockSidebarBtn");
const burger = document.getElementById("hamburgerBtn");

// Click burger → lock/unlock sidebar
burger.addEventListener("click", () =>
{
    locked = !locked;

    if (locked) {
        sidebar.classList.add("notion-locked");
        app.classList.add("notion-shift");
    } else {
        sidebar.classList.remove("notion-locked");
        app.classList.remove("notion-shift");
    }
});
