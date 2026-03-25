if (deleteBtn) {
    deleteBtn.addEventListener("click", function () {

        const ids = getSelectedIds();

        if (ids.length === 0) return;

        if (!confirm("Are you sure you want to delete selected employees?")) return;

        window.location.href = `/Employee/BulkDelete?ids=${ids.join(",")}`;
    });
}