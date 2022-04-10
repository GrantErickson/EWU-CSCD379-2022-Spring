import { WordleGame, GameState } from '@/scripts/wordleGame'

describe('Game Test', () => {
  test('is an instance', () => {
    const game = new WordleGame('APPLE')
    expect(game).toBeTruthy()
  })
  test('Win Game', () => {
    const game = new WordleGame('APPLE')
    expect(game.state).toBe(GameState.Active)
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('L')
    game.submitWord()
    expect(game.state).toBe(GameState.Active)

    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('L')
    game.currentWord.addLetter('E')
    game.submitWord()
    expect(game.state).toBe(GameState.Won)
  })

  test('Lose Game', () => {
    const game = new WordleGame('APPLE')
    expect(game.state).toBe(GameState.Active)
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('L')
    game.submitWord()
    expect(game.state).toBe(GameState.Active)

    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('L')
    game.submitWord()
    expect(game.state).toBe(GameState.Active)

    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('L')
    game.submitWord()
    expect(game.state).toBe(GameState.Active)

    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('L')
    game.submitWord()
    expect(game.state).toBe(GameState.Active)

    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('P')
    game.currentWord.addLetter('A')
    game.currentWord.addLetter('L')
    game.submitWord()
    expect(game.state).toBe(GameState.Lost)
  })
})
