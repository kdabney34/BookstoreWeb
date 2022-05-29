import canvasConfetti from 'https://cdn.skypack.dev/canvas-confetti';

function Party() {
    startConfetti();
    confetti();
}

document.getElementById('confetti').addEventListener('click', Party)