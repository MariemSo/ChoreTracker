document.addEventListener("DOMContentLoaded", function () {
    // Function to apply hover effect
    function applyHoverEffect(buttonClass, hoverText, hoverClass) {
      var buttons = document.getElementsByClassName(buttonClass);

      Array.from(buttons).forEach(function(button) {
        var defaultText = button.innerText;

        button.addEventListener("mouseover", function () {
          button.classList.remove('btn-primary');
          button.classList.add(hoverClass);
          button.innerText = hoverText;
        });

        button.addEventListener("mouseout", function () {
          button.classList.remove(hoverClass);
          button.classList.add('btn-primary');
          button.innerText = defaultText;
        });
      });
    }

    // Applying hover effect to View Buttons
    applyHoverEffect('viewButton', 'View Details', 'btn-success');

    // Applying hover effect to Add Buttons
    applyHoverEffect('addButton', 'Add to Favorites', 'btn-info');

    // Applying hover effect to Edit Buttons
    applyHoverEffect('editButton', 'Edit Job', 'btn-warning');

    // Applying hover effect to Cancel Buttons
    applyHoverEffect('cancelButton', 'Cancel Job', 'btn-secondary');

    applyHoverEffect('doneButton', 'Done Job', 'btn-secondary');
});