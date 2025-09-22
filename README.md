# Conway's Game of Life - Full Stack Application 🧬

A modern, interactive implementation of Conway's Game of Life with a .NET 8 API backend and React + TypeScript frontend.

## 🌟 Features

### 🎮 Interactive Game Board
- **Click-to-toggle cells** - Click any cell to toggle between alive/dead states
- **Zoom & Pan** - Mouse wheel to zoom, Ctrl+drag to pan around large boards
- **Responsive grid** - Automatically adjusts cell size based on board dimensions
- **Smooth animations** - Optional cell transition animations for visual appeal

### 🎛️ Advanced Controls
- **Play/Pause simulation** with adjustable speed (0.1x to 20x)
- **Step-by-step evolution** - Advance one generation at a time
- **Bulk advancement** - Jump ahead N generations instantly
- **Final state detection** - Automatically find stable or oscillating patterns
- **Undo/Redo system** with full history tracking

### 📚 Pattern Library
- **Famous Conway patterns** organized by category:
  - **Still Lifes**: Block, Bee-hive, Loaf
  - **Oscillators**: Blinker, Toad, Beacon, Pulsar
  - **Spaceships**: Glider, LWSS, MWSS, HWSS
  - **Guns**: Gosper Glider Gun
  - **Methuselahs**: R-pentomino, Diehard, Acorn
- **Pattern preview** with visual thumbnails
- **One-click insertion** - Center patterns on your board
- **Search & filter** by name, category, or discoverer

### 💾 Import/Export & Sharing
- **JSON export/import** - Save and load board states
- **Shareable URLs** - Generate links to share specific boards
- **Local storage** - Automatic save of current board and settings
- **Drag & drop** file support for easy importing

### ⚡ Performance & Analytics
- **Real-time metrics** - Generation time, API response time, cells per second
- **Large board support** - Efficiently handles boards up to 1000×1000 cells
- **Memory optimization** - Uses BitArray for compact cell storage
- **Performance hints** - Automatic suggestions for large boards

### 🎯 Developer Experience
- **Comprehensive API** - RESTful endpoints with full OpenAPI documentation
- **TypeScript types** - Full type safety matching .NET DTOs
- **Error handling** - Robust error boundaries and user-friendly messages
- **Keyboard shortcuts** - Power user features for faster interaction
- **Docker support** - Complete containerized deployment

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 20+ (for frontend)
- Docker & Docker Compose (optional)

### Option 1: Docker Compose (Recommended)
```bash
# Clone the repository
git clone https://github.com/youruser/life-api.git
cd life-api

# Start all services
docker-compose -f ops/docker-compose.yml up --build

# Access the application
# Frontend: http://localhost:3000
# API: http://localhost:5000
# Database: localhost:1433
```

### Option 2: Manual Setup

#### Backend (.NET API)
```bash
# Navigate to API project
cd src/Life.Api

# Restore packages
dotnet restore

# Run the API
dotnet run
# API will be available at http://localhost:5000
```

#### Frontend (React)
```bash
# Navigate to web project
cd web

# Install dependencies
npm install

# Start development server
npm run dev
# Frontend will be available at http://localhost:3000
```

## 🏗️ Architecture

### Backend - Clean Architecture with DDD
```
src/
├── Life.Api/              # API layer - Controllers, middleware, configuration
├── Life.Application/      # Application layer - CQRS commands/queries, handlers
├── Life.Domain/          # Domain layer - Aggregates, entities, business rules
└── Life.Infrastructure/  # Infrastructure - Data access, external services
```

**Key Technologies:**
- **.NET 8** - Latest LTS framework with native AOT support
- **Clean Architecture** - Separation of concerns with proper dependency inversion
- **Domain-Driven Design** - Rich domain models with encapsulated business logic
- **CQRS + MediatR** - Command/Query separation for scalability
- **Entity Framework Core** - Code-first database with migrations
- **Conway's Rules Engine** - Strategy pattern for extensible cellular automaton rules

### Frontend - Modern React Stack
```
web/
├── src/
│   ├── components/       # React components (GameBoard, Controls, etc.)
│   ├── hooks/           # Custom React hooks (useGameOfLife, useApi)
│   ├── services/        # API client with axios and error handling
│   ├── types/           # TypeScript interfaces matching .NET DTOs
│   ├── constants/       # Conway patterns and configuration
│   └── App.tsx          # Main application component
├── public/              # Static assets
└── dist/               # Built application
```

**Key Technologies:**
- **React 18** - Modern React with concurrent features
- **TypeScript** - Full type safety and IntelliSense
- **Vite** - Fast build tool and dev server
- **TanStack Query** - Server state management and caching
- **Tailwind CSS** - Utility-first styling with custom components
- **Lucide React** - Modern icon library

## 🎮 How to Play

### Basic Controls
1. **Start with a pattern** - Use the Pattern Library or click cells manually
2. **Press Play** - Watch your pattern evolve according to Conway's rules
3. **Adjust speed** - Use the speed slider (0.1x to 20x speed)
4. **Experiment** - Try different patterns and see how they behave

### Conway's Rules
1. **Birth**: Dead cell with exactly 3 neighbors becomes alive
2. **Survival**: Live cell with 2-3 neighbors stays alive
3. **Death**: All other cells die or stay dead

### Keyboard Shortcuts
- `Space` - Play/Pause
- `→` - Next generation
- `R` - Randomize board
- `C` - Clear board
- `Ctrl+Z` - Undo
- `Ctrl+Y` - Redo
- `+/-` - Zoom in/out
- `0` - Reset zoom
- `Arrow keys` - Pan view

### Advanced Features
- **Final State Detection** - Automatically detect when patterns stabilize
- **History Navigation** - Travel back through previous generations
- **Pattern Insertion** - Add famous patterns to your board
- **Board Sharing** - Generate URLs to share interesting configurations

## 🔧 API Endpoints

### Board Operations
```http
POST /boards          # Upload a new board configuration
POST /boards/next     # Get the next generation
POST /boards/nahead   # Advance N generations
POST /boards/final    # Get the final stable state
```

### Request/Response Format
```typescript
// Board representation
interface BoardDto {
  width: number;
  height: number;
  aliveCells: Position[];
  generation: number;
}

interface Position {
  x: number;
  y: number;
}
```

### Example Usage
```bash
# Upload a glider pattern
curl -X POST http://localhost:5000/boards \
  -H "Content-Type: application/json" \
  -d '{
    "board": {
      "width": 10,
      "height": 10,
      "aliveCells": [
        {"x": 1, "y": 2},
        {"x": 2, "y": 3},
        {"x": 3, "y": 1},
        {"x": 3, "y": 2},
        {"x": 3, "y": 3}
      ],
      "generation": 0
    }
  }'
```

## 🧪 Testing

### Backend Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Life.Domain.Tests
dotnet test tests/Life.Api.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Tests (Future Enhancement)
```bash
# Run unit tests
npm test

# Run e2e tests
npm run test:e2e
```

## 🐳 Docker Deployment

### Development
```bash
docker-compose -f ops/docker-compose.yml up --build
```

### Production
```bash
docker-compose -f ops/docker-compose.prod.yml up -d
```

### Services
- **Web**: React frontend on port 3000
- **API**: .NET backend on port 5000
- **Database**: SQL Server on port 1433

## 📊 Performance

### Backend Performance
- **Algorithm**: O(width × height) per generation
- **Memory**: BitArray storage (8x more efficient than bool array)
- **Concurrency**: Thread-safe operations with proper locking
- **Caching**: Response caching for repeated requests

### Frontend Performance
- **Rendering**: Virtual scrolling for large boards
- **State Management**: Optimized React Query caching
- **Bundle Size**: Code splitting and tree shaking
- **Memory**: Efficient cell lookup with Set data structures

### Benchmarks (Approximate)
- **Small boards** (50×30): ~1ms per generation
- **Medium boards** (200×200): ~50ms per generation
- **Large boards** (1000×1000): ~2s per generation
- **Memory usage**: ~1MB per 100k cells

## 🎨 Customization

### Adding New Patterns
```typescript
// In src/constants/patterns.ts
export const CUSTOM_PATTERNS = {
  myPattern: {
    name: 'My Custom Pattern',
    positions: [
      { x: 0, y: 0 },
      { x: 1, y: 1 },
      // ... more positions
    ],
    description: 'My awesome pattern',
    category: 'custom'
  }
};
```

### Custom Cellular Automaton Rules
```csharp
// In Life.Domain/Rules/
public class MyCustomRules : CellularAutomatonRulesBase
{
    public override bool ShouldCellBeAlive(bool currentState, int aliveNeighbors)
    {
        // Implement your custom rules here
        return aliveNeighbors == 3 || (currentState && aliveNeighbors == 2);
    }
}
```

## 🚧 Roadmap

### Upcoming Features
- [ ] **WebSocket real-time updates** for collaborative editing
- [ ] **Multiple rule sets** - Support for different cellular automaton rules
- [ ] **Board templates** - Predefined board sizes and configurations
- [ ] **Pattern evolution tracking** - Visualize how patterns change over time
- [ ] **Mobile optimizations** - Touch-friendly controls and gestures
- [ ] **Performance profiling** - Built-in performance analysis tools

### Potential Enhancements
- [ ] **3D visualization** - Three.js integration for 3D Conway boards
- [ ] **Sound effects** - Audio feedback for pattern changes
- [ ] **Pattern marketplace** - Community sharing of interesting patterns
- [ ] **Machine learning** - Pattern classification and prediction
- [ ] **Multi-player mode** - Collaborative board editing

## 🤝 Contributing

### Development Setup
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass: `dotnet test && npm test`
6. Commit with conventional commits: `git commit -m "feat: add amazing feature"`
7. Push and create a Pull Request

### Code Style
- **Backend**: Follow Microsoft C# conventions with EditorConfig
- **Frontend**: ESLint + Prettier with TypeScript strict mode
- **Tests**: Arrange-Act-Assert pattern with descriptive test names
- **Documentation**: Keep README and code comments up to date

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **John Conway** - Creator of the Game of Life
- **Bill Gosper** - Discoverer of the famous Gosper Glider Gun
- **LifeWiki** - Comprehensive database of Conway patterns
- **React Team** - Amazing frontend framework
- **Microsoft** - .NET platform and excellent tooling

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/youruser/life-api/issues)
- **Discussions**: [GitHub Discussions](https://github.com/youruser/life-api/discussions)
- **Email**: your.email@example.com

---

**Made with ❤️ and lots of ☕**

*Conway's Game of Life: Where simple rules create infinite complexity* 🌌