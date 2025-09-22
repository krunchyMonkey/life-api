import { CONWAY_PATTERNS } from '../../constants/patterns';

export type CategoryType = keyof typeof CONWAY_PATTERNS;

export const categoryLabels: Record<CategoryType, string> = {
  stillLifes: 'Still Lifes',
  oscillators: 'Oscillators', 
  spaceships: 'Spaceships',
  guns: 'Guns',
  methuselahs: 'Methuselahs'
};

export const categoryDescriptions: Record<CategoryType, string> = {
  stillLifes: 'Patterns that remain unchanged',
  oscillators: 'Patterns that repeat after N generations',
  spaceships: 'Patterns that move across the board',
  guns: 'Patterns that create other patterns',
  methuselahs: 'Long-lived patterns that eventually stabilize'
};