// Write your JavaScript code.

/**
 * Copy phone number to clipboard and open Zalo chat
 * @param {string} phone The phone number to copy
 */
function copyAndOpenZalo(phone) {
    if (!phone) return;
    
    // Copy to clipboard
    navigator.clipboard.writeText(phone).then(() => {
        // Visual feedback (optional: could use a toast)
        console.log("Số điện thoại đã được copy: " + phone);
        
        // Open Zalo web/app link
        // Zalo.me link format: https://zalo.me/[phone]
        const zaloUrl = "https://zalo.me/" + phone.replace(/[^0-9]/g, "");
        window.open(zaloUrl, "_blank");
    }).catch(err => {
        console.error("Lỗi khi copy: ", err);
        // Fallback: still try to open Zalo
        const zaloUrl = "https://zalo.me/" + phone.replace(/[^0-9]/g, "");
        window.open(zaloUrl, "_blank");
    });
}
