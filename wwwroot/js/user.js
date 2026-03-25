
    const editBtn = document.getElementById("editBtn");
    const deleteBtn = document.getElementById("deleteBtn");
    const selectAll = document.getElementById("selectAll");

    function getSelected() {
        return Array.from(document.querySelectorAll(".user-checkbox:checked"));
    }

    function updateButtons() {
        const selected = getSelected();

    editBtn.disabled = true;
    deleteBtn.disabled = true;

    if (selected.length === 1) {
            const role = selected[0].dataset.role;

    editBtn.disabled = false;

    // Admin cannot be deleted
    if (role !== "Admin") {
        deleteBtn.disabled = false;
            }
        }

        if (selected.length > 1) {
        editBtn.disabled = true;

            // Check if any admin selected
            const hasAdmin = selected.some(cb => cb.dataset.role === "Admin");

    deleteBtn.disabled = hasAdmin; // disable if admin included
        }
    }

    // Checkbox change
    document.addEventListener("change", function (e) {
        if (e.target.classList.contains("user-checkbox")) {
        updateButtons();
        }
    });

    // Select all
    selectAll.addEventListener("change", function () {
        document.querySelectorAll(".user-checkbox").forEach(cb => {
            cb.checked = this.checked;
        });
    updateButtons();
    });

    // 🔥 Edit click
    editBtn.addEventListener("click", function () {
        const selected = getSelected();
    if (selected.length === 1) {
            const id = selected[0].dataset.id;
    window.location.href = `/Admin/EditUser/${id}`;
        }
    });

    // 🔥 Delete click
    deleteBtn.addEventListener("click", function () {
        const selected = getSelected();
        const ids = selected.map(cb => cb.dataset.id);

        if (ids.length > 0) {
            if (confirm("Are you sure to delete selected users?")) {
        window.location.href = `/Admin/DeleteMultiple?ids=${ids.join(",")}`;
            }
        }
    });

    // 🔍 Search
    document.getElementById("searchInput").addEventListener("keyup", function () {
        let filter = this.value.toLowerCase();
    let rows = document.querySelectorAll("#usersTable tbody tr");

        rows.forEach(row => {
        let text = row.innerText.toLowerCase();
    row.style.display = text.includes(filter) ? "" : "none";
        });
    });

