import React from 'react';
import { CONWAY_PATTERNS } from '../../../constants/patterns';
import type { CategoryType } from '../constants';

interface CategoryTabsProps {
  selectedCategory: CategoryType;
  onCategoryChange: (category: CategoryType) => void;
  categoryLabels: Record<CategoryType, string>;
  categoryDescriptions: Record<CategoryType, string>;
}

export const CategoryTabs: React.FC<CategoryTabsProps> = ({
  selectedCategory,
  onCategoryChange,
  categoryLabels,
  categoryDescriptions
}) => {
  const categories = Object.keys(CONWAY_PATTERNS) as Array<CategoryType>;

  return (
    <div className="mb-3">
      <div className="flex flex-wrap gap-2 mb-3">
        {categories.map(category => (
          <button
            key={category}
            onClick={() => onCategoryChange(category)}
            className={`px-4 py-2 text-sm rounded-full transition-all duration-200 font-medium ${
              selectedCategory === category
                ? 'btn-primary'
                : 'btn'
            }`}
          >
            {categoryLabels[category]}
          </button>
        ))}
      </div>

      <p className="text-sm text-gray-600 mt-2">
        {categoryDescriptions[selectedCategory]}
      </p>
    </div>
  );
};