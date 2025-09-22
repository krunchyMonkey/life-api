/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Conway's Game of Life color scheme
        cell: {
          alive: '#22c55e',      // green-500
          dead: '#f3f4f6',       // gray-100
          'alive-hover': '#16a34a', // green-600
          'dead-hover': '#e5e7eb'   // gray-200
        },
        pattern: {
          glider: '#3b82f6',     // blue-500
          oscillator: '#f59e0b', // amber-500
          'still-life': '#8b5cf6' // violet-500
        }
      },
      animation: {
        'pulse-slow': 'pulse 3s infinite',
        'cell-birth': 'cellBirth 0.3s ease-out',
        'cell-death': 'cellDeath 0.3s ease-in'
      },
      keyframes: {
        cellBirth: {
          '0%': { transform: 'scale(0)', opacity: '0' },
          '50%': { transform: 'scale(1.2)', opacity: '0.7' },
          '100%': { transform: 'scale(1)', opacity: '1' }
        },
        cellDeath: {
          '0%': { transform: 'scale(1)', opacity: '1' },
          '50%': { transform: 'scale(0.8)', opacity: '0.3' },
          '100%': { transform: 'scale(1)', opacity: '0' }
        }
      },
      gridTemplateColumns: {
        'board': 'repeat(var(--board-width), minmax(0, 1fr))',
      },
      spacing: {
        'cell': '8px', // Standard cell size
      }
    },
  },
  plugins: [],
}