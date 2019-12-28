import React, { Component } from 'react';
import axios from 'axios';

export default class JobInputs extends Component {

    constructor(props) {

        super(props);

        this.state = {};

        this.change = this.change.bind(this);
        this.submit = this.submit.bind(this);
    }

    change = (event) => {

        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        this.setState({
            [name]: value
        });

    }

    submit = (event) => {

        event.preventDefault();

        axios.post(`https://localhost:44349/api/${this.props.workspace}/complete-job/${this.props.jobId}`, this.state)
            .then(res => {

                // SignalR will take care of the rest

            })

    }

    render() {
        return (
            <form>
                {this.props.job.Inputs.map((input, index) => {
                    switch (input.Type) {
                        case 'string':
                            return (
                                <div key={index}>
                                    <label htmlFor={'input' + index}>{input.Name}</label>
                                    <input id={'input' + index} type="text" name={input.Name} value={this.state[input.Name]} onChange={this.change} />
                                </div>
                            );
                        case 'select':
                            return (
                                <div className="form-group" key={index}>
                                    <label htmlFor={'input' + index}>{input.Name}</label>
                                    <select id={'input' + index} name={input.Name} onChange={this.change}>
                                        <option value="" selected>Choose...</option>
                                        {input.Options.map((option, index) => {
                                            return <option value={option}>{option}</option>
                                        })}
                                    </select>
                                </div>
                            );
                        default:
                            return null;
                    }
                })}
                <div className="form-control">
                    <button onClick={this.submit}>Respond</button>
                </div>
            </form>
        );
    }

}