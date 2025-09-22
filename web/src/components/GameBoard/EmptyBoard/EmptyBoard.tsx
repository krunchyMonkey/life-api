interface EmptyBoardProps {
  className?: string;
}

export const EmptyBoard: React.FC<EmptyBoardProps> = ({ className = '' }) => (
  <div className={`info-card flex items-center justify-center min-h-[200px] ${className}`}>
    <div className="text-center">
      <div className="text-gray-400 text-6xl mb-3">🏁</div>
      <p className="text-gray-700 font-medium text-lg">No board loaded</p>
      <p className="text-gray-500 mt-2">Upload a board or create a new one to get started</p>
    </div>
  </div>
);