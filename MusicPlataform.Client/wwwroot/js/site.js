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

// Reproductor de audio en el footer

document.addEventListener('DOMContentLoaded', () => {
    const audioFooter = document.getElementById('audio-footer');
    const audioElement = document.getElementById('footer-audio');
    const footerImg = document.getElementById('footer-img');
    const footerTitle = document.getElementById('footer-title');
    const footerArtist = document.getElementById('footer-artist');
    const footerPlayBtn = document.getElementById('footer-play');
    const progressBar = document.getElementById('progress-bar');
    const currentTimeSpan = document.getElementById('current-time');
    const totalDurationSpan = document.getElementById('total-duration');

    let lastActiveButton = null;

    window.playTrack = function (audioUrl, title, artist, imgSrc) {
        audioFooter.classList.remove('d-none');

        footerTitle.textContent = title;
        footerArtist.textContent = artist;
        footerImg.src = imgSrc;

        audioElement.src = audioUrl;
        audioElement.play();

        footerPlayBtn.innerHTML = '<i class="bi bi-pause-fill"></i>';

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
            if (lastActiveButton) {
                lastActiveButton.innerHTML = '<i class="bi bi-play-fill"></i>';
            }
        };
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
});



