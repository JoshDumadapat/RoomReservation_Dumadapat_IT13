window.preventNonNumeric = function(element) {
    if (!element) return;
    
    // Use a data attribute to track if we've already set up this element
    if (element.dataset.numericSetup === 'true') {
        return;
    }
    element.dataset.numericSetup = 'true';
    
    // Store original handlers if any
    var keydownHandler = function(e) {
        var key = e.key;
        var keyCode = e.keyCode || e.which;
        
        // Allow: backspace (8), delete (46), tab (9), escape (27), enter (13)
        // Allow: home (36), end (35), left (37), up (38), right (39), down (40)
        var allowedKeys = [8, 9, 13, 27, 35, 36, 37, 38, 39, 40, 46];
        
        // Allow Ctrl+A (65), Ctrl+C (67), Ctrl+V (86), Ctrl+X (88)
        if (e.ctrlKey && [65, 67, 86, 88].indexOf(keyCode) !== -1) {
            return true;
        }
        
        // Allow navigation keys
        if (allowedKeys.indexOf(keyCode) !== -1) {
            return true;
        }
        
        // Allow numbers on main keyboard (48-57) and numpad (96-105)
        if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105)) {
            // Check if value would exceed 11 digits
            var currentValue = element.value || '';
            if (currentValue.length >= 11 && keyCode !== 8 && keyCode !== 46) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            return true;
        }
        
        // Check if it's a digit character (0-9)
        if (key && key.length === 1 && /[0-9]/.test(key)) {
            var currentValue = element.value || '';
            if (currentValue.length >= 11) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            return true;
        }
        
        // Prevent everything else
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
        return false;
    };
    
    element.addEventListener('keydown', keydownHandler, true);
    
    var keypressHandler = function(e) {
        // Double check on keypress
        var charCode = e.which || e.keyCode;
        if (charCode < 48 || (charCode > 57 && charCode < 96) || charCode > 105) {
            if (charCode !== 8 && charCode !== 9 && charCode !== 13 && charCode !== 27 && charCode !== 46) {
                e.preventDefault();
                return false;
            }
        }
    };
    
    element.addEventListener('keypress', keypressHandler, true);
    
    var pasteHandler = function(e) {
        e.preventDefault();
        var paste = (e.clipboardData || window.clipboardData).getData('text');
        // Only allow digits
        var digitsOnly = paste.replace(/\D/g, '');
        // Limit to 11 digits
        if (digitsOnly.length > 11) {
            digitsOnly = digitsOnly.substring(0, 11);
        }
        element.value = digitsOnly;
        // Trigger input event for Blazor
        var inputEvent = new Event('input', { bubbles: true, cancelable: true });
        element.dispatchEvent(inputEvent);
    };
    
    element.addEventListener('paste', pasteHandler, true);
    
    var inputHandler = function(e) {
        // Filter out any non-numeric characters that might have gotten through
        var value = element.value;
        var digitsOnly = value.replace(/\D/g, '');
        if (digitsOnly.length > 11) {
            digitsOnly = digitsOnly.substring(0, 11);
        }
        if (value !== digitsOnly) {
            element.value = digitsOnly;
            // Trigger input event again for Blazor
            var inputEvent = new Event('input', { bubbles: true, cancelable: true });
            element.dispatchEvent(inputEvent);
        }
    };
    
    element.addEventListener('input', inputHandler, true);
    
    return element;
};

