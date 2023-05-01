import { Expression, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class FieldGet extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly fieldName: WithPosition<string>,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.findField(context) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.target.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.target.validateBlock(context));

    if (errors.length === 0 && !this.findField(context)) {
      errors.push({
        status: "error",
        message: `Undefined field ${this.fieldName.value} for type ${
          this.target.resultType(context).name
        }`,
        start: this.fieldName.start,
        end: this.fieldName.end,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
    semanticTokens.push({
      type: "property",
      start: this.fieldName.start,
      length: this.fieldName.end.offset - this.fieldName.start.offset,
    });
  }

  private findField(context: BlockContext): TO2Type | undefined {
    const targetType = this.target
      .resultType(context)
      .realizedType(context.module);

    return targetType.findField(this.fieldName.value);
  }
}