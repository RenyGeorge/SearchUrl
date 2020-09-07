import React, { Component } from 'react';
import Autocomplete from 'react-toolbox/lib/autocomplete';
import styles from './style.scss';
export class Home extends Component {

    constructor(props){
    super(props);
    this.state = {
        searchEngines: [],
        selectedSearchEngine: "Google",
        searchString: "",
        searchUrl: "",
        result:""
        }
        this.searchEngine = {};
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    componentDidMount() {
        this.fetchData();
    }
    fetchData = () => {
        fetch('api/Search/Get')
            .then(response => response.json())
            .then(response => {
                if (response && response.searchConfigs.length > 0) {
                    response.searchConfigs.forEach(searchConfig => {
                        this.searchEngine[searchConfig.name] = searchConfig.name;
                    })
                }
                this.setState({ searchEngines: response.searchConfigs });
                
            });
    }
    handleChange(event) {
       
        console.log(event);
        this.setState({ [event.target.name]: event.target.value });
    }
    handleSubmit (event){
        event.preventDefault();
        let formData = new FormData();
        formData.append('searchEngine', this.state.selectedSearchEngine);
        formData.append('searchString', this.state.searchString);
        formData.append('searchUrl', this.state.searchUrl);
        
        fetch('api/Search/SearchResult', {
            method: 'post',
            body: formData
        }).then(response => response.json())
            .then(response => {
            this.setState({ result: response });
        });

    }
  displayName = Home.name

    render() {
        let themeWithDisabledOption = {suggestions: styles['disabled-suggestions']};
      return (
          <form onSubmit={this.handleSubmit}>
              <Autocomplete
                  value={this.state.selectedSearchEngine}
                  source={this.searchEngine}
                  queryable="true"
                  multiple={false}
                  renderItem={(item, isHighlighted) =>
                      <div style={{ background: isHighlighted ? 'lightgray' : 'white' }}>
                          {item}
                      </div>
                  }
                  onChange={(value, event) => this.setState({ selectedSearchEngine: value })}
                  direction="down"
                  selectedPosition="above"
                  name="selectedSearchEngine"
                  menuStyle={styles['menuStyle']}
                  inputProps={styles['menuStyle']}
                  openOnFocus={true}

          />
              <label>
                  Search String
          <input type="text" value={this.state.searchString} onChange={this.handleChange} name="searchString" />
              </label>

              <label>
                  Search Url
                    <input type="text" value={this.state.searchUrl} onChange={this.handleChange} name="searchUrl" />
              </label>
              <input type="submit" value="Submit" />
              <br/>
              <label>{this.state.result}</label>
          </form>
        
    );
  }
}
