import React from 'react';
import './Messages.scss';

interface MessageProps {
	datetime: Date;
	children: string;
}

function getTimeString(date: Date) {
	const hours = date.getHours().toString().padStart(2, '0');
	const minutes = date.getMinutes().toString().padStart(2, '0');
	return `${hours}:${minutes}`;
}

export const Message: React.FC<MessageProps> = ({ children, datetime }) => {
	return (
		<div className="message message--other">
			<p className="message__content">{children}</p>
			<span className="message__time">{getTimeString(datetime)}</span>
		</div>
	);
};
