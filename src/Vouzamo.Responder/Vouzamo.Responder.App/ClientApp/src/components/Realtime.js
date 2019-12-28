import React, { Component } from 'react';
import * as SignalR from '@aspnet/signalr';
import axios from 'axios';
import JobInputs from './JobInputs.js';

export class Realtime extends Component {

    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            workspace: '',
            jobs: {},
            activeJob: ''
        };

        this.onJobSubmitted = this.onJobSubmitted.bind(this);
        this.onJobCompleted = this.onJobCompleted.bind(this);
        this.selectJob = this.selectJob.bind(this);
    }

    componentDidMount = () => {
        const workspace = window.prompt('Your workspace:', 'demo');

        const hubConnection = new SignalR.HubConnectionBuilder()
            .withUrl('/hub')
            .configureLogging(SignalR.LogLevel.Trace)
            .build();

        hubConnection.on("JobSubmitted", this.onJobSubmitted);
        hubConnection.on("JobCompleted", this.onJobCompleted);

        this.setState({ hubConnection, workspace }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :('));
        });
    }

    onJobSubmitted = (id, job) => {

        console.log('Job Submitted:');
        console.log(id);
        console.log(job);

        var model = JSON.parse(job);

        console.log(model);

        if (model.Workspace === this.state.workspace) {
            var jobs = this.state.jobs;

            jobs[id] = model;

            this.setState({
                jobs: jobs
            }, () => console.log(this.state.jobs))
        }
    }

    onJobCompleted = (key) => {

        var jobs = this.state.jobs;
        var activeJob = this.state.activeJob;

        if (Object.keys(jobs).includes(key)) {

            delete jobs[key];

            if (activeJob === key) {
                activeJob = '';
            }

            this.setState({
                jobs: jobs,
                activeJob: activeJob
            });

        }

    }

    selectJob = (key) => {

        this.setState({
            activeJob: key
        });

    }

    render() {
        return (
            <div>
                <h1>Realtime</h1>
                {Object.keys(this.state.jobs).length === 0 &&
                    <div>
                        <h2>No pending jobs</h2>
                    <p>To add a job, simply visit <a href={`/proxy/${this.state.workspace}/`} target="_blank" rel="noopener noreferrer">{ `/proxy/${this.state.workspace}/*` }</a>.</p>
                    </div>
                }
                <ul className="list-group">
                    {Object.keys(this.state.jobs).map((key, index) => {
                        var job = this.state.jobs[key];
                        var className = this.state.activeJob === key ? 'list-group-item active' : 'list-group-item';

                        return (
                            <li key={key} className={className} onClick={() => this.selectJob(key)}>
                                {key}
                                <br />
                                {job.Request.Path.Value}
                            </li>
                        );
                    })}
                </ul>
                {this.state.activeJob !== '' &&
                    <div>
                        <h2>Respond to job</h2>
                        <JobInputs workspace={this.state.workspace} job={this.state.jobs[this.state.activeJob]} jobId={this.state.activeJob} />
                    </div>
                }
            </div>
        );
    }
}
