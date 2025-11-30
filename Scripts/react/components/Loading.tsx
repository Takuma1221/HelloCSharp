import React from 'react';

interface Props {
    isLoading: boolean;
}

/**
 * ローディングオーバーレイコンポーネント
 */
export const LoadingOverlay: React.FC<Props> = ({ isLoading }) => {
    if (!isLoading) return null;

    return (
        <div className="loading-overlay">
            <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        </div>
    );
};
