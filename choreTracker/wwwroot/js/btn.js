document.addEventListener("DOMContentLoaded", function () {
    // Function to apply hover effect
    function applyHoverEffect(buttonClass, hoverText, hoverClass) {
      var buttons = document.getElementsByClassName(buttonClass);

      Array.from(buttons).forEach(function(button) {


        button.addEventListener("mouseover", function () {
          button.classList.remove('btn-dark');
          button.classList.add(hoverClass);
        });

        button.addEventListener("mouseout", function () {
          button.classList.remove(hoverClass);
          button.classList.add('btn-dark');
        });
      });
    }

    // Applying hover effect to View Buttons
    applyHoverEffect('viewButton', 'Take A Pick!', 'btn-success');

    // Applying hover effect to Add Buttons
    applyHoverEffect('addButton', 'Yes You Can!', 'btn-info');

    // Applying hover effect to Edit Buttons
    applyHoverEffect('editButton', 'Oups! Again!', 'btn-warning');

    // Applying hover effect to Cancel Buttons
    applyHoverEffect('cancelButton', 'How Sad!', 'btn-secondary');

    applyHoverEffect('doneButton', "Oh You'r Rich!", 'btn-info');
});