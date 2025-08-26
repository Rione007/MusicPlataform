// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Buscar por nombre

const searchInput = document.querySelector('.search');

searchInput.addEventListener('input', () => {
    const query = searchInput.value.toLowerCase().trim();
    const albums = document.querySelectorAll('.artists-row');

    albums.forEach(album => {
        const title = album.querySelector('.title')?.textContent.toLowerCase() || '';
        if (title.includes(query)) {
            album.style.display = '';
        } else {
            album.style.display = 'none';
        }
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


// Footer 
const audioFooter = document.getElementById('audio-footer');
const audioElement = document.getElementById('footer-audio');
const footerImg = document.getElementById('footer-img');
const footerTitle = document.getElementById('footer-title');
const footerArtist = document.getElementById('footer-artist');
const footerPlayBtn = document.getElementById('footer-play');
const progressBar = document.getElementById('progress-bar');
const currentTimeSpan = document.getElementById('current-time');
const totalDurationSpan = document.getElementById('total-duration');

function playTrack(audioUrl, title, artist, imgSrc) {
    audioFooter.style.display = 'flex'; 

    footerTitle.textContent = title;
    footerArtist.textContent = artist;
    footerImg.src = imgSrc;

    audioElement.src = audioUrl;
    audioElement.play();

    footerPlayBtn.textContent = '❚❚'; 

    audioElement.onloadedmetadata = () => {
        totalDurationSpan.textContent = formatTime(audioElement.duration);
        progressBar.max = audioElement.duration;
    };

    audioElement.ontimeupdate = () => {
        progressBar.value = audioElement.currentTime;
        currentTimeSpan.textContent = formatTime(audioElement.currentTime);
    };

    audioElement.onended = () => {
        footerPlayBtn.textContent = '▶';
    };
}

function togglePlay() {
    if (audioElement.paused) {
        audioElement.play();
        footerPlayBtn.textContent = '❚❚';
    } else {
        audioElement.pause();
        footerPlayBtn.textContent = '▶';
    }
}

function seekAudio(event) {
    audioElement.currentTime = event.target.value;
}

function formatTime(seconds) {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
}