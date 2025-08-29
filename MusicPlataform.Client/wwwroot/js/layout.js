document.addEventListener('DOMContentLoaded', function () {
    async function loadUserPlaylists() {
        const listEl = document.getElementById('playlists-list');
        if (!listEl) return;
        try {
            const resp = await fetch('/Playlists/MyPlaylistsJson', { credentials: 'same-origin' });
            if (!resp.ok) {
                listEl.innerHTML = '<li class="nav-item text-danger"><span class="nav-link">Error</span></li>';
                return;
            }
            const data = await resp.json();
            const items = data.items || [];
            if (items.length === 0) {
                listEl.innerHTML = '<li class="nav-item text-muted"><span class="nav-link">Sin listas</span></li>';
            } else {
                listEl.innerHTML = items.map(p =>
                    `<li class="nav-item"><a class="nav-link" href="/Playlists/Details/${p.id}" data-playlist-id="${p.id}">${p.name}</a></li>`
                ).join('');

            }
        } catch {
            listEl.innerHTML = '<li class="nav-item text-danger"><span class="nav-link">Error</span></li>';
        }
    }

    const btnCreate = document.getElementById('btn-create-playlist');
    if (btnCreate) {
        btnCreate.addEventListener('click', async () => {
            btnCreate.disabled = true;
            btnCreate.innerText = 'Creando...';
            try {
                const resp = await fetch('/Playlists/CreateDefault', {
                    method: 'POST',
                    credentials: 'same-origin'
                });
                const data = await resp.json();
                if (resp.ok && data && data.id) {
                    window.location.href = data.url || `/Playlists/Details/${data.id}`;
                } else {
                    alert(data?.message || 'No se pudo crear la lista');
                }
            } catch {
                alert('Error de red');
            } finally {
                btnCreate.disabled = false;
                btnCreate.innerText = '+ Crear lista';
            }
        });
    }

   

    loadUserPlaylists();
});
