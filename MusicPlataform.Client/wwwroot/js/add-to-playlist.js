document.addEventListener('DOMContentLoaded', async () => {
    const dropdowns = document.querySelectorAll('.playlist-dropdown');

    for (const dropdown of dropdowns) {
        const trackId = dropdown.dataset.trackId;
        try {
            const resp = await fetch('/Playlists/MyPlaylistsJson', { credentials: 'same-origin' });
            const data = await resp.json();
            const items = data.items || [];

            if (items.length === 0) {
                dropdown.innerHTML = `<li><span class="dropdown-item text-muted">Sin listas</span></li>`;
            } else {
                dropdown.innerHTML = items.map(p => `
                    <li>
                        <button class="dropdown-item" onclick="addToPlaylist(${p.id}, ${trackId}, this)">
                            ${p.name}
                        </button>
                    </li>
                `).join('');
            }
        } catch (e) {
            dropdown.innerHTML = `<li><span class="dropdown-item text-danger">Error</span></li>`;
        }
    }
});

async function addToPlaylist(playlistId, trackId, btn) {
    if (btn) {
        btn.disabled = true;
        btn.textContent = 'Agregando...';
    }
    try {
        const resp = await fetch(`/Playlists/AddTrack?id=${playlistId}&trackId=${trackId}`, {
            method: 'POST',
            credentials: 'same-origin'
        });
        if (!resp.ok) {
            alert('No se pudo agregar');
        } else {
            alert('Agregado a la lista');
        }
    } catch {
        alert('Error de red');
    } finally {
        if (btn) {
            btn.disabled = false;
            btn.textContent = btn.dataset.originalText || 'Agregar';
        }
    }
}
