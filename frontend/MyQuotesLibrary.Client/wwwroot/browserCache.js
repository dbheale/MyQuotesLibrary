const DB_NAME = 'MyQuotesLibraryDb';
const STORE_NAME = 'sqlite_blobs';

// Initialize IndexedDB
async function initIndexedDB() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, 1);
        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            if (!db.objectStoreNames.contains(STORE_NAME)) {
                db.createObjectStore(STORE_NAME);
            }
        };
        request.onsuccess = () => resolve(request.result);
        request.onerror = (event) => reject(event.target.error);
    });
}

// Generate download link from IndexedDB
export async function generateDownloadLink(parent, file) {

    const backupPath = `${file}`;
    const res = await getBlobFromIndexedDb(file);

    if (res) {
        const a = document.createElement("a");
        a.href = URL.createObjectURL(res);
        a.download = backupPath;
        a.target = "_self";
        a.innerText = `Download ${backupPath}`;
        parent.innerHTML = '';
        parent.appendChild(a);
        return true;
    }

    return false;
}
export async function saveDataToIndexedDb(file, data) {
    const db = await initIndexedDB();
    const transaction = db.transaction(STORE_NAME, 'readwrite');
    const store = transaction.objectStore(STORE_NAME);

    try {
        const blob = new Blob([data], { type: 'application/octet-stream' });

        const request = store.put(blob, file);
        await new Promise((resolve, reject) => {
            request.onsuccess = () => {
                console.log(`Stored ${file} in IndexedDB. Size: ${data.byteLength} bytes`);
                resolve(1);
            };
            request.onerror = () => reject(request.error);
        });

        console.log(`Data size before Blob creation: ${data.byteLength}`);

        return 1;

    } catch (error) {
        console.error("Failed to save to IndexedDB:", error);
    }

    return 0;
}


async function getBlobFromIndexedDb(file) {
    const inexedDb = await initIndexedDB();
    const readTransaction = inexedDb.transaction(STORE_NAME, 'readonly');
    const store = readTransaction.objectStore(STORE_NAME);

    // Wrap the request in a Promise to await its result
    const result = await new Promise((resolve, reject) => {
        const request = store.get(file);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
    return result;
}

export async function getDataFromIndexedDb(file) {
    const blob = await getBlobFromIndexedDb(file);
    if (blob && blob instanceof Blob) {
        const arrayBuffer = await blob.arrayBuffer();
        console.log(`Restored Blob as ArrayBuffer. Length: ${arrayBuffer.byteLength}`);
        const uint8Array = new Uint8Array(arrayBuffer);
        let binary = '';
        for (let i = 0; i < uint8Array.byteLength; i++) {
            binary += String.fromCharCode(uint8Array[i]);
        }
        return btoa(binary);  // Convert to Base64
    }
    return null;
}