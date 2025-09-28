// Calendar Integration JavaScript Functions

window.downloadFile = (filename, contentType, content) => {
    // Convert content to Blob
    const blob = new Blob([content], { type: contentType });
    
    // Create download link
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    
    // Trigger download
    document.body.appendChild(a);
    a.click();
    
    // Cleanup
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
};

window.openCalendarUrl = (url, target = '_blank') => {
    window.open(url, target);
};

window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch (err) {
        console.error('Failed to copy text: ', err);
        return false;
    }
};

// Calendar Integration utilities
window.calendarIntegration = {
    // Check if browser supports calendar APIs
    isSupported: () => {
        return 'navigator' in window && 'clipboard' in navigator;
    },
    
    // Format date for different calendar systems
    formatDate: (dateString, format = 'iso') => {
        const date = new Date(dateString);
        switch (format) {
            case 'google':
                return date.toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z';
            case 'outlook':
                return date.toISOString();
            case 'apple':
                return date.toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z';
            default:
                return date.toISOString();
        }
    },
    
    // Show notification
    showNotification: (title, message, type = 'info') => {
        // You can integrate with your notification system here
        console.log(`${type.toUpperCase()}: ${title} - ${message}`);
    }
};