// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Buscar por nombre

const searchInput = document.querySelector('.search');

searchInput.addEventListener('input', () => {
    const query = searchInput.value.toLowerCase().trim();
    const albums = document.querySelectorAll('.album');

    albums.forEach(album => {
        const title = album.querySelector('.title')?.textContent.toLowerCase() || '';
        if (title.includes(query)) {
            album.style.display = '';
        } else {
            album.style.display = 'none';
        }
    });
});

// Mostrar detalles
function showDetails(title, img, year, artistId) {
    document.getElementById("details").innerHTML = `
        <h3>${title}</h3>
        <img src="${img}" alt="${title}">
        <p><strong>Año:</strong> ${year}</p>
        <p><strong>ID del Artista:</strong> ${artistId}</p>
        <p>Más información de la canción...</p>
    `;
}


// Footer 

const footer = document.getElementById('audio-footer');
const footerImg = document.getElementById('footer-img');
const footerTitle = document.getElementById('footer-title');
const footerArtist = document.getElementById('footer-artist');
const footerPlayBtn = document.getElementById('footer-play');
const currentTimeEl = document.getElementById('current-time');
const totalDurationEl = document.getElementById('total-duration');
const progressBar = document.getElementById('progress-bar'); 

let isSeeking = false;

function showFooter(title, artist, img, audio) {
    footer.style.display = 'flex';
    footerImg.src = img;
    footerTitle.textContent = title;
    footerArtist.textContent = artist;
    footerPlayBtn.innerHTML = '&#10073;&#10073;';
    console.log("Mostrando footer con:", title, artist);

    if (audio.readyState >= 1) {
        totalDurationEl.textContent = formatTime(audio.duration);
        progressBar.max = audio.duration;
    } else {
        audio.addEventListener('loadedmetadata', () => {
            totalDurationEl.textContent = formatTime(audio.duration);
            progressBar.max = audio.duration;
        }, { once: true });
    }

    footerPlayBtn.onclick = () => {
        if (audio.paused) {
            audio.play();
            footerPlayBtn.innerHTML = '&#10073;&#10073;';
            if (currentButton) currentButton.innerHTML = '&#10073;&#10073;';
        } else {
            audio.pause();
            footerPlayBtn.innerHTML = '&#9658;';
            if (currentButton) currentButton.innerHTML = '&#9658;';
        }
    };

    audio.ontimeupdate = () => {
        if (!isSeeking) {
            progressBar.value = audio.currentTime;
            currentTimeEl.textContent = formatTime(audio.currentTime);
        }
    };

    progressBar.oninput = (e) => {
        isSeeking = true;
        currentTimeEl.textContent = formatTime(e.target.value);
    };

    progressBar.onchange = (e) => {
        audio.currentTime = e.target.value;
        isSeeking = false;
    };
}

function hideFooter() {
    footer.style.display = 'none';
}

function formatTime(seconds) {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}


