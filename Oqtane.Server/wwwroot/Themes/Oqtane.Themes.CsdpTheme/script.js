function openFileInNewTab(fileBase64, contentType) {
    const byteCharacters = atob(fileBase64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
    URL.revokeObjectURL(url);
}

/**
 * Downloads a file from a base64 encoded string
 * @param {string} fileBase64 - The base64 encoded file data
 * @param {string} fileName - The name of the file to download
 * @param {string} contentType - The content type of the file
 */
function downloadFileFromBase64(fileBase64, fileName, contentType) {
    // Convert base64 to blob
    const byteCharacters = atob(fileBase64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });
    
    // Create URL for the blob
    const url = URL.createObjectURL(blob);
    
    // Create download link
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName || 'download'; // Use provided filename or default
    document.body.appendChild(link); // Required for Firefox
    
    // Trigger download
    link.click();
    
    // Cleanup
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}