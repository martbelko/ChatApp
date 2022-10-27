import React from 'react';
import { useRecoilValue } from 'recoil';
import { userIdState } from '../../atoms';
import './Messages.scss';

interface Props
{
    authorId: number;
    content: string;
    datetime: Date;
}

export const Message: React.FC<Props> = ({ authorId, content, datetime }) => {
    const hours = datetime.getHours();
    const mins = datetime.getMinutes();
    const userId = useRecoilValue(userIdState);
    return (
        <div className={"message " + (userId == authorId ? 'message--me' : 'message--other')}>
            <p className="message__content">{content}</p>
            <span className="message__time">{hours + ':' + mins}</span>
        </div>
    );
}
