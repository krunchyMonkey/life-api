import React, { memo } from 'react';
import { AlertCircle } from 'lucide-react';

interface ErrorPanelProps {
  error: string | null;
}

export const ErrorPanel: React.FC<ErrorPanelProps> = memo(({ error }) => {
  if (!error) return null;

  return (
    <div className="bg-red-50 border border-red-200 rounded-lg p-3">
      <div className="flex items-start gap-2">
        <AlertCircle size={16} className="text-red-500 mt-0.5" />
        <div>
          <p className="text-sm font-medium text-red-800">Error</p>
          <p className="text-sm text-red-600">{error}</p>
        </div>
      </div>
    </div>
  );
});