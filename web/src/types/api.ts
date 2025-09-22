// TypeScript types that mirror the .NET API DTOs

export interface Position {
  x: number;
  y: number;
}

export interface BoardDimensions {
  width: number;
  height: number;
}

// Frontend board representation (for UI components)
export interface BoardDto {
  width: number;
  height: number;
  aliveCells: Position[];
  generation?: number;
}

// API DTOs (match the .NET API exactly)
export interface ApiBoardDto {
  width: number;
  height: number;
  alive: number[][]; // Array of [x, y] coordinate pairs
}

export interface UploadRequest {
  width: number;
  height: number;
  alive: number[][]; // Array of [x, y] coordinate pairs
}

export interface UploadResponse {
  boardId: string;
}

export interface NextRequest {
  boardId: string;
}

export interface NAheadRequest {
  boardId: string;
  n: number;
}

export interface FinalRequest {
  boardId: string;
}

export interface FinalResponse {
  board: ApiBoardDto;
  stable: boolean;
  cyclic: boolean;
  iterations: number;
  cycleLength?: number;
}

// API Response types
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

export interface ErrorResponse {
  title: string;
  status: number;
  detail: string;
  instance?: string;
  type?: string;
}

// UI State types
export interface GameState {
  board: BoardDto | null;
  isPlaying: boolean;
  speed: number; // generations per second
  history: BoardDto[];
  maxHistory: number;
}

export interface PatternTemplate {
  name: string;
  description: string;
  category: 'stillLife' | 'oscillator' | 'spaceship' | 'gun' | 'custom';
  pattern: Position[];
  width: number;
  height: number;
  period?: number; // for oscillators
}

// Utility types for the board
export type CellState = boolean;
export type BoardGrid = CellState[][];

// Conway's Game of Life specific types
export interface ConwayPattern {
  name: string;
  positions: Position[];
  description: string;
  discoverer?: string;
  year?: number;
}

// Performance monitoring
export interface PerformanceMetrics {
  generationTime: number; // ms
  renderTime: number; // ms
  apiResponseTime: number; // ms
  cellsCount: number;
  generationsPerSecond: number;
  lastUpdate?: number; // timestamp for throttling
}

// Local Storage types
export interface SavedBoard {
  id: string;
  name: string;
  board: BoardDto;
  created: string;
  modified: string;
}

// Export/Import types
export interface ExportData {
  board: BoardDto;
  metadata: {
    exported: string;
    version: string;
    generator: 'Conway Game of Life Web App';
  };
}