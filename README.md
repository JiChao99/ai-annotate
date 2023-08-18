# ai-annotate

annotate by GAI, use Semantic Kernel, a blazor site.

## demo

input

```md
The current wave of LLMs default to conversational natural language — languages that humans communicate in like English. Parsing natural language is an extremely difficult task, no matter how much you pamper a prompt with rules like "respond in the form a bulleted list". Natural language might have structure, but it's hard for typical software to reconstruct it from raw text.
```

output 

auto tags, annotate

```md
#NLP #software

The current wave of LLMs[^1] default to conversational natural language[^2] — languages that humans communicate in like English. Parsing[^3] natural language is an extremely difficult task, no matter how much you pamper a prompt with rules like "respond in the form a bulleted list". Natural language might have structure, but it's hard for typical software[^4] to reconstruct it from raw text.

[^1]: LLMs refers to large language models, which are machine learning models that are trained on vast amounts of text data to generate human-like language output. Examples include GPT-3 and BERT. [source](https://en.wikipedia.org/wiki/Large_language_model)
[^2]: Conversational natural language refers to natural language that is used in everyday conversations between humans, such as English, Spanish, or French. 
[^3]: Parsing refers to the process of analyzing a sentence or phrase into its grammatical components, such as nouns, verbs, and adjectives.
[^4]: Software refers to computer programs that perform specific tasks, such as word processing, data analysis, or web browsing.
```

## TODO

- [ ] use support markdown annotate component
- [ ] multi level
- [ ] multi output style: markdown,text,html
- [ ] automatic scraping, media information.

---

- [ ] 使用支持Markdown注释的组件。
- [ ] 多层级，支持不同级别的注释
- [ ] 多输出风格: markdown,text,html
- [ ] 自动刮削，媒体信息
