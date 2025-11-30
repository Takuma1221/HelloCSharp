import React, { useEffect, useState } from 'react';

interface ToastMessage {
    id: number;
    message: string;
    type: 'success' | 'danger';
}

interface Props {
    messages: ToastMessage[];
    onRemove: (id: number) => void;
}

/**
 * トースト通知コンポーネント
 */
export const ToastContainer: React.FC<Props> = ({ messages, onRemove }) => {
    return (
        <div className="toast-container position-fixed top-0 end-0 p-3">
            {messages.map((msg) => (
                <Toast key={msg.id} {...msg} onClose={() => onRemove(msg.id)} />
            ))}
        </div>
    );
};

interface ToastProps extends ToastMessage {
    onClose: () => void;
}

const Toast: React.FC<ToastProps> = ({ message, type, onClose }) => {
    useEffect(() => {
        const timer = setTimeout(onClose, 3000);
        return () => clearTimeout(timer);
    }, [onClose]);

    return (
        <div
            className={`toast show align-items-center text-white bg-${type} border-0`}
            role="alert"
        >
            <div className="d-flex">
                <div className="toast-body">{message}</div>
                <button
                    type="button"
                    className="btn-close btn-close-white me-2 m-auto"
                    onClick={onClose}
                />
            </div>
        </div>
    );
};

// トーストIDを生成するためのカウンター
let toastId = 0;

/**
 * トースト通知を管理するカスタムフック
 */
export const useToast = () => {
    const [messages, setMessages] = useState<ToastMessage[]>([]);

    const showToast = (message: string, type: 'success' | 'danger') => {
        const id = ++toastId;
        setMessages((prev) => [...prev, { id, message, type }]);
    };

    const removeToast = (id: number) => {
        setMessages((prev) => prev.filter((m) => m.id !== id));
    };

    return {
        messages,
        showSuccess: (message: string) => showToast(message, 'success'),
        showError: (message: string) => showToast(message, 'danger'),
        removeToast,
    };
};
