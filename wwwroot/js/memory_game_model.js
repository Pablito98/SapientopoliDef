// memory_game_model.js

class MemoryGameModel {
    constructor() {
        this.symbols = ['🍎', '🍌', '🍒', '🍇', '🍉', '🍓', '🍍', '🥝'];
        this.cards = [...this.symbols, ...this.symbols];
        this.cards = this.cards.sort(() => Math.random() - 0.5);
        this.firstCard = null;
        this.secondCard = null;
        this.lockBoard = false;
    }

    flipCard(card) {
        if (this.lockBoard) return;
        if (card === this.firstCard) return;

        card.classList.add('flipped');
        if (!this.firstCard) {
            this.firstCard = card;
            return;
        }

        this.secondCard = card;
        this.lockBoard = true;

        this.checkMatch();
    }

    checkMatch() {
        let isMatch = this.firstCard.dataset.symbol === this.secondCard.dataset.symbol;
        isMatch ? this.disableCards() : this.unflipCards();
    }

    disableCards() {
        this.firstCard.removeEventListener('click', this.firstCard.clickHandler);
        this.secondCard.removeEventListener('click', this.secondCard.clickHandler);
        this.resetBoard();
    }

    unflipCards() {
        setTimeout(() => {
            this.firstCard.classList.remove('flipped');
            this.secondCard.classList.remove('flipped');
            this.resetBoard();
        }, 1000);
    }

    resetBoard() {
        [this.firstCard, this.secondCard] = [null, null];
        this.lockBoard = false;
    }
}
