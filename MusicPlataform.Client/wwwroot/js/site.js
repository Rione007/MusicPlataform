// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Búsqueda en tiempo real

document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('search-input');

    searchInput.addEventListener('input', () => {
        const query = searchInput.value.toLowerCase().trim();

        // Filtrar artistas
        document.querySelectorAll('.artist-item').forEach(item => {
            const name = item.querySelector('.artist-name')?.textContent.toLowerCase() || '';
            const bio = item.querySelector('.artist-bio')?.textContent.toLowerCase() || '';

            if (name.includes(query) || bio.includes(query)) {
                item.style.display = '';
            } else {
                item.style.display = 'none';
            }
        });

        // Filtrar canciones
        document.querySelectorAll('.track-item').forEach(item => {
            const title = item.querySelector('.track-title')?.textContent.toLowerCase() || '';
            const artist = item.querySelector('.track-artist')?.textContent.toLowerCase() || '';

            if (title.includes(query) || artist.includes(query)) {
                item.style.display = '';
            } else {
                item.style.display = 'none';
            }
        });
    });
});


//// Mostrar detalles
//function showDetails(title, img, year, artistId) {
//    document.getElementById("details").innerHTML = `
//        <h3>${title}</h3>
//        <img src="${img}" alt="${title}">
//        <p><strong>Año:</strong> ${year}</p>
//        <p><strong>ID del Artista:</strong> ${artistId}</p>
//        <p>Más información de la canción...</p>
//    `;
//}

// Reproducir canciones --> DAVID

document.addEventListener('DOMContentLoaded', async () => {
    const audioFooter = document.getElementById('audio-footer');
    const audioElement = document.getElementById('footer-audio');
    const footerImg = document.getElementById('footer-img');
    const footerTitle = document.getElementById('footer-title');
    const footerArtist = document.getElementById('footer-artist');
    const footerPlayBtn = document.getElementById('footer-play');
    const progressBar = document.getElementById('progress-bar');
    const currentTimeSpan = document.getElementById('current-time');
    const totalDurationSpan = document.getElementById('total-duration');
    const prevBtn = document.getElementById('footer-prev');
    const nextBtn = document.getElementById('footer-next');

    let lastActiveButton = null;
    let allTracks = [];
    let currentTrackIndex = -1;
    let audioHistory = [];
    let shuffledIndices = [];
    let shuffledPos = 0;

    async function loadAllTracks() {
        try {
            const response = await fetch('https://localhost:7106/api/tracks');
            if (!response.ok) throw new Error(`Error al cargar canciones: ${response.status}`);

            const data = await response.json();

            allTracks = data.map(track => ({
                audioUrl: location.origin + track.audioUrl,  
                title: track.title,
                artist: track.artist,
                imgSrc: '/img/default-imagen.webp'
            }));

            console.log('Canciones cargadas:', allTracks);

            if (allTracks.length > 0 && nextBtn) nextBtn.disabled = false;
            if (prevBtn) prevBtn.disabled = true;

        } catch (error) {
            console.error('Error al cargar canciones:', error);
        }
    }

    await loadAllTracks();

    function shuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    }

    function initShuffle() {
        shuffledIndices = allTracks.map((_, i) => i);
        shuffleArray(shuffledIndices);
        shuffledPos = 0;
    }

    window.playTrack = function (audioUrl, title, artist, imgSrc) {
        // Restaurar ícono del último botón activo
        if (lastActiveButton) {
            lastActiveButton.innerHTML = '<i class="bi bi-play-fill"></i>';
            lastActiveButton = null;
        }

        // Buscar el índice de la canción actual
        const newIndex = allTracks.findIndex(t => t.audioUrl === audioUrl);
        if (currentTrackIndex !== -1 && newIndex !== currentTrackIndex) {
            audioHistory.push(currentTrackIndex);
            if (prevBtn) prevBtn.disabled = false;
        }
        currentTrackIndex = newIndex;

        // Reproducir audio
        audioFooter.classList.remove('d-none');
        footerTitle.textContent = title;
        footerArtist.textContent = artist;
        footerImg.src = imgSrc || '/img/default-imagen.webp';
        audioElement.src = audioUrl;
        audioElement.play();

        footerPlayBtn.innerHTML = '<i class="bi bi-pause-fill"></i>';

        // Buscar y actualizar el botón de la tarjeta que corresponde
        document.querySelectorAll('.play-button').forEach(button => {
            const btnAudio = location.origin + button.getAttribute('data-audio');
            if (btnAudio === audioUrl) {
                button.innerHTML = '<i class="bi bi-pause-fill"></i>';
                lastActiveButton = button;
            } else {
                button.innerHTML = '<i class="bi bi-play-fill"></i>';
            }
        });

        // Tiempo y progreso
        audioElement.onloadedmetadata = () => {
            totalDurationSpan.textContent = formatTime(audioElement.duration);
            progressBar.max = audioElement.duration;
        };

        audioElement.ontimeupdate = () => {
            progressBar.value = audioElement.currentTime;
            currentTimeSpan.textContent = formatTime(audioElement.currentTime);
        };

        audioElement.onended = () => {
            footerPlayBtn.innerHTML = '<i class="bi bi-play-fill"></i>';
            playRandomTrack();
        };
    };


    window.playPreviousTrack = function () {
        if (audioHistory.length > 0) {
            const prevIndex = audioHistory.pop();
            const prevTrack = allTracks[prevIndex];
            playTrack(prevTrack.audioUrl, prevTrack.title, prevTrack.artist, prevTrack.imgSrc);
            currentTrackIndex = prevIndex;

            if (audioHistory.length === 0 && prevBtn) {
                prevBtn.disabled = true;
            }
        }
    };

    window.playRandomTrack = function () {
        if (allTracks.length === 0) return;

        if (shuffledIndices.length === 0) {
            initShuffle();
        }

        let attempts = 0;
        while (attempts < shuffledIndices.length) {
            if (shuffledPos >= shuffledIndices.length) {
                initShuffle();
            }

            const nextIndex = shuffledIndices[shuffledPos];
            shuffledPos++;

            if (nextIndex !== currentTrackIndex) {
                const track = allTracks[nextIndex];
                playTrack(track.audioUrl, track.title, track.artist, track.imgSrc);
                return;
            }
            attempts++;
        }

        const track = allTracks[currentTrackIndex];
        playTrack(track.audioUrl, track.title, track.artist, track.imgSrc);
    };


    window.handlePlayClick = function (btn) {
        const audioUrl = btn.getAttribute('data-audio');
        const title = btn.getAttribute('data-title');
        const artist = btn.getAttribute('data-artist');
        const imgSrc = '/img/default-imagen.webp';

        const fullAudioSrc = location.origin + audioUrl;

        if (audioElement.src === fullAudioSrc) {
            if (audioElement.paused) {
                audioElement.play();
                footerPlayBtn.innerHTML = '<i class="bi bi-pause-fill"></i>';
                btn.innerHTML = '<i class="bi bi-pause-fill"></i>';
            } else {
                audioElement.pause();
                footerPlayBtn.innerHTML = '<i class="bi bi-play-fill"></i>';
                btn.innerHTML = '<i class="bi bi-play-fill"></i>';
            }
        } else {
            if (lastActiveButton) {
                lastActiveButton.innerHTML = '<i class="bi bi-play-fill"></i>';
            }

            playTrack(audioUrl, title, artist, imgSrc);
            btn.innerHTML = '<i class="bi bi-pause-fill"></i>';
            lastActiveButton = btn;
        }
    };

    window.togglePlay = function () {
        if (audioElement.paused) {
            audioElement.play();
            footerPlayBtn.innerHTML = '<i class="bi bi-pause-fill"></i>';
            if (lastActiveButton) {
                lastActiveButton.innerHTML = '<i class="bi bi-pause-fill"></i>';
            }
        } else {
            audioElement.pause();
            footerPlayBtn.innerHTML = '<i class="bi bi-play-fill"></i>';
            if (lastActiveButton) {
                lastActiveButton.innerHTML = '<i class="bi bi-play-fill"></i>';
            }
        }
    };

    window.seekAudio = function (event) {
        audioElement.currentTime = event.target.value;
    };

    function formatTime(seconds) {
        const mins = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }

    document.querySelectorAll('.play-button').forEach(button => {
        button.addEventListener('click', function () {
            window.handlePlayClick(this);
        });
    });

    const volumeControl = document.getElementById('volume-control');
    if (volumeControl) {
        volumeControl.addEventListener('input', () => {
            audioElement.volume = volumeControl.value;
        });
    }
});



// Playlist --> LEYTON

window.handlePlayLink = function (a) {
    const audioUrl = a.getAttribute('data-audio');
    const title = a.getAttribute('data-title');
    const artist = a.getAttribute('data-artist') || '';
    playTrack(audioUrl, title, artist, '/img/default-imagen.webp');
};


// Este abrirá un modal para seleccionar la playlist:

window.openPlaylistSelector = async function (trackId) {
    const response = await fetch('/Playlists/MyPlaylistsJson');
    if (!response.ok) {
        alert('No se pudo cargar tus playlists');
        return;
    }

    const data = await response.json();
    const playlists = data.items;

    let html = '<div class="modal-body">';
    html += '<p>Selecciona una playlist:</p>';
    html += '<ul class="list-group">';
    playlists.forEach(pl => {
        html += `<li class="list-group-item list-group-item-action" onclick="addTrackToPlaylist(${pl.id}, ${trackId})">${pl.name} ${pl.isPublic ? '(pública)' : '(privada)'}</li>`;
    });
    html += '</ul>';
    html += '</div>';

    html += `<div class="modal-footer">
                <button class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
            </div>`;

    document.getElementById('playlistModalContent').innerHTML = html;
    const modal = new bootstrap.Modal(document.getElementById('playlistModal'));
    modal.show();
};




// Crear método para agregar el track a una playlist


window.addTrackToPlaylist = async function (playlistId, trackId) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    try {
        const response = await fetch(`/Playlists/AddTrack?id=${playlistId}&trackId=${trackId}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            }
        });

        if (response.ok) {
            alert('Canción agregada a la lista.');
            bootstrap.Modal.getInstance(document.getElementById('playlistModal')).hide();
        } else {
            const error = await response.json();
            alert(error.message || 'No se pudo agregar.');
        }
    } catch (e) {
        alert('Error de red.');
    }
};
