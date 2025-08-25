// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showDetails(title, artist, img) {
    document.getElementById("details").innerHTML = `
        <h3>${title}</h3>
        <p><strong>Artista:</strong> ${artist}</p>
        <img src="${img}" alt="">
        <p>Más información de la canción...</p>
    `;
}
