import React from 'react';
import './Messages.scss';

export function Message(): JSX.Element {
    return (
        <div className="message message--other">
            <p className="message__content">This is my message</p>
            <span className="message__time">16:10</span>
        </div>
    );
}
