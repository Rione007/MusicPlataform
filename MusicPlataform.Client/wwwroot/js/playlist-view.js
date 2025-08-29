(async function () {
    async function loadPlaylist() {
        const ul = document.getElementById('playlist-tracks');
        try {
            const resp = await fetch(`/Playlists/PlaylistJson?id=${playlistId}`, { credentials: 'same-origin' });
            const data = await resp.json();
            const items = data.items || [];
            if (items.length === 0) {
                ul.innerHTML = '<li class="list-group-item bg-dark text-muted">Todavía no hay canciones.</li>';
                return;
            }
            ul.innerHTML = items.map(it => `
                <li class="list-group-item bg-dark text-white d-flex justify-content-between align-items-center">
                    <div>
                        <div class="fw-semibold">${it.title}</div>
                        <small class="text-white">${it.artist ?? ''}</small>
                    </div>
                    <div>
                        <button class="btn btn-sm btn-outline-light me-2"
                                onclick="playFooter('${encodeURIComponent(it.audioUrl)}', '${it.title}', '${it.artist ?? ''}', '/img/default-imagen.webp')">
                            <i class="bi bi-play-fill"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="removeFromList(${playlistId}, ${it.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </li>
            `).join('');
        } catch (e) {
            ul.innerHTML = '<li class="list-group-item bg-dark text-danger">Error al cargar.</li>';
            console.error(e);
        }
    }

 

    window.addToList = async function (playlistId, trackId, btn) {
        if (btn) { btn.disabled = true; btn.innerText = 'Agregando...'; }
        const resp = await fetch(`/Playlists/AddTrack?id=${playlistId}&trackId=${trackId}`, {
            method: 'POST', credentials: 'same-origin'
        });
        if (!resp.ok) alert('No se pudo agregar');
        await loadPlaylist();
        if (btn) { btn.disabled = false; btn.innerText = 'Agregar'; }
    };

    window.removeFromList = async function (playlistId, trackId) {
        const resp = await fetch(`/Playlists/RemoveTrack?id=${playlistId}&trackId=${trackId}`, {
            method: 'POST', credentials: 'same-origin'
        });
        if (!resp.ok) alert('No se pudo quitar');
        await loadPlaylist();
    };

    window.playFooter = function (url, title, artist, imgSrc) {
        try {
            const audio = document.getElementById('footer-audio');
            const audioFooter = document.getElementById('audio-footer');
            const footerTitle = document.getElementById('footer-title');
            const footerArtist = document.getElementById('footer-artist');
            const footerImg = document.getElementById('footer-img');
            const footerPlayBtn = document.getElementById('footer-play');
            const progressBar = document.getElementById('progress-bar');
            const currentTimeSpan = document.getElementById('current-time');
            const totalDurationSpan = document.getElementById('total-duration');

            audioFooter.classList.remove('d-none');
            footerTitle.textContent = title;
            footerArtist.textContent = artist;
            footerImg.src = imgSrc;

            audio.src = decodeURIComponent(url);
            audio.play();
            footerPlayBtn.innerHTML = '<i class="bi bi-pause-fill"></i>';

            audio.onloadedmetadata = () => {
                totalDurationSpan.textContent = formatTime(audio.duration);
                progressBar.max = audio.duration;
            };
            audio.ontimeupdate = () => {
                progressBar.value = audio.currentTime;
                currentTimeSpan.textContent = formatTime(audio.currentTime);
            };
            audio.onended = () => {
                footerPlayBtn.innerHTML = '<i class="bi bi-play-fill"></i>';
            };

            function formatTime(seconds) {
                const mins = Math.floor(seconds / 60);
                const secs = Math.floor(seconds % 60);
                return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
            }
        } catch (e) { console.error(e); }
    };


    await loadPlaylist();
 
})();
