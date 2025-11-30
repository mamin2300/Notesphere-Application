document.addEventListener("DOMContentLoaded", () => {

    const input = document.getElementById("todoInput");
    const addBtn = document.getElementById("addTodoBtn");
    const list = document.getElementById("todoList");

    function addTodo() {
        const text = input.value.trim();
        if (text === "") return;

        const li = document.createElement("li");
        li.classList.add("todo-item");

        li.innerHTML = `
            <div class="todo-left">
                <input type="checkbox" class="todo-checkbox" />
                <span class="todo-label">${text}</span>
            </div>
            <span class="todo-delete">&times;</span>
        `;

        list.appendChild(li);
        input.value = "";
    }

    addBtn.addEventListener("click", addTodo);

    input.addEventListener("keypress", (e) => {
        if (e.key === "Enter") addTodo();
    });

    list.addEventListener("click", (e) => {
        if (e.target.classList.contains("todo-delete")) {
            e.target.parentElement.remove();
        }

        if (e.target.classList.contains("todo-checkbox")) {
            const label = e.target.nextElementSibling;
            label.classList.toggle("todo-checked");
        }
    });
});
