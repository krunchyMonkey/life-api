import React from 'react';
import { AlertCircle } from 'lucide-react';

interface ApiErrorBannerProps {
  error: Error | null;
}

export const ApiErrorBanner: React.FC<ApiErrorBannerProps> = ({ error }) => {
  if (!error) return null;

  return (
    <div className="bg-red-50 border-b border-red-200 px-4 py-2">
      <div className="max-w-7xl mx-auto flex items-center gap-2 text-red-700">
        <AlertCircle size={16} />
        <span className="text-sm">
          Unable to connect to API. Some features may be limited.
        </span>
      </div>
    </div>
  );
};