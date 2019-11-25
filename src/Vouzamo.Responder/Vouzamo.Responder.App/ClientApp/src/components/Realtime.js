import React, { Component } from 'react';
import * as SignalR from '@aspnet/signalr';

export class Realtime extends Component {

    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            workspace: ''
        };

        this.onJobSubmitted = this.onJobSubmitted.bind(this);
    }

    componentDidMount = () => {
        const workspace = window.prompt('Your workspace:', 'demo');

        const hubConnection = new SignalR.HubConnectionBuilder()
            .withUrl('/hub')
            .configureLogging(SignalR.LogLevel.Trace)
            .build();

        hubConnection.on("JobSubmitted", this.onJobSubmitted);

        this.setState({ hubConnection, workspace }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :('));
        });
    }

    onJobSubmitted(id) {
        console.log('Job Submitted:');
        console.log(id);
    }

    render() {
        return (
            <div>
                <h1>Realtime</h1>
                <p>See console (for now)</p>
            </div>
        );
    }
}
