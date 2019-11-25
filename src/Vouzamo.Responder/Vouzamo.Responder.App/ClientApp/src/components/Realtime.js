import React, { Component } from 'react';
import HubConnectionBuilder, { LogLevel } from '@aspnet/signalr-client';

export class Realtime extends Component {

    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            workspace: ''
        };
    }

    componentDidMount = () => {
        const workspace = window.prompt('Your workspace:', 'demo');

        const hubConnection = new HubConnectionBuilder()
            .withUrl('/hub')
            .configureLogging(LogLevel.Information)
            .build();

        this.setState({ hubConnection, workspace }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :('));

            this.state.hubConnection.on('JobSubmitted', (id, job) => {
                console.log(id);
                console.log(job);
            });
        });
    }

    render() {
        return (
            <div>
                <h1>Counter</h1>
                <p>See console (for now)</p>
            </div>
        );
    }
}
